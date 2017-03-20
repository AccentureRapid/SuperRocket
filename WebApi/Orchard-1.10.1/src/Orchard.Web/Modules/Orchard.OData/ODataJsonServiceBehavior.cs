
namespace Orchard.OData
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The jsonp support inspector.
    /// </summary>
    internal class JSONPSupportInspector : IDispatchMessageInspector
    {
        /// <summary>
        /// The encoding.
        /// Assume utf-8, note that Data Services supports
        /// charset negotation, so this needs to be more
        /// sophisticated (and per-request) if clients will 
        /// use multiple charsets
        /// </summary>
        private static readonly Encoding encoding = Encoding.UTF8;

        #region IDispatchMessageInspector Members

        /// <summary>
        /// The behavior after receive request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="channel">
        /// The channel.
        /// </param>
        /// <param name="instanceContext">
        /// The instance context.
        /// </param>
        /// <returns>
        /// The optional callback object.
        /// </returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            if (request.Properties.ContainsKey("UriTemplateMatchResults"))
            {
                var httpmsg = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
                var match = (UriTemplateMatch)request.Properties["UriTemplateMatchResults"];

                string format = match.QueryParameters["$format"];
                if ("json".Equals(format, StringComparison.InvariantCultureIgnoreCase))
                {
                    // strip out $format from the query options to avoid an error
                    // due to use of a reserved option (starts with "$")
                    match.QueryParameters.Remove("$format");

                    // replace the Accept header so that the Data Services runtime 
                    // assumes the client asked for a JSON representation
                    httpmsg.Headers["Accept"] = "application/json, text/plain;q=0.5";
                    httpmsg.Headers["Accept-Charset"] = "utf-8";

                    string callback = match.QueryParameters["$callback"];
                    if (!string.IsNullOrEmpty(callback))
                    {
                        match.QueryParameters.Remove("$callback");
                        return callback;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The before send reply.
        /// </summary>
        /// <param name="reply">
        /// The reply.
        /// </param>
        /// <param name="correlationState">
        /// The correlation state.
        /// </param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (correlationState != null && correlationState is string)
            {
                // if we have a JSONP callback then buffer the response, wrap it with the
                // callback call and then re-create the response message
                var callback = (string)correlationState;

                bool bodyIsText = false;
                var response = reply.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
                if (response != null)
                {
                    string contentType = response.Headers["Content-Type"];
                    if (contentType != null)
                    {
                        // Check the response type and change it to text/javascript if we know how.
                        if (contentType.StartsWith("text/plain", StringComparison.InvariantCultureIgnoreCase))
                        {
                            bodyIsText = true;
                            response.Headers["Content-Type"] = "text/javascript;charset=utf-8";
                        }
                        else if (contentType.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase))
                        {
                            response.Headers["Content-Type"] = contentType.Replace("application/json", "text/javascript");
                        }
                    }
                }

                XmlDictionaryReader reader = reply.GetReaderAtBodyContents();
                reader.ReadStartElement();

                string content = encoding.GetString(reader.ReadContentAsBase64());
                if (bodyIsText)
                {
                    // Escape the body as a string literal.
                    content = "\"" + QuoteJScriptString(content) + "\"";
                }

                content = callback + "(" + content + ")";

                Message newreply = Message.CreateMessage(MessageVersion.None, string.Empty, new Writer(content));
                newreply.Properties.CopyProperties(reply.Properties);

                reply = newreply;
            }
        }

        #endregion

        /// <summary>
        /// Quotes the j script string.
        /// </summary>
        /// <param name="s">
        /// The j script string provided.
        /// </param>
        /// <returns>
        /// The quote j script string.
        /// </returns>
        private static string QuoteJScriptString(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            StringBuilder builder = null;
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];
                if (((((ch == '\r') || (ch == '\t')) || ((ch == '"') || (ch == '\\'))) ||
                     (((ch == '\n') || (ch < ' ')) || ((ch > '\x007f') || (ch == '\b')))) || (ch == '\f'))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(s.Length + 6);
                    }

                    if (count > 0)
                    {
                        builder.Append(s, startIndex, count);
                    }

                    startIndex = i + 1;
                    count = 0;
                }

                switch (ch)
                {
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    default:
                        if ((ch < ' ') || (ch > '\x007f'))
                        {
                            builder.AppendFormat(CultureInfo.InvariantCulture, @"\u{0:x4}", (int) ch);
                        }
                        else
                        {
                            count++;
                        }

                        break;
                }
            }

            string result;
            if (builder == null)
            {
                result = s;
            }
            else
            {
                if (count > 0)
                {
                    builder.Append(s, startIndex, count);
                }

                result = builder.ToString();
            }

            return result;
        }

        #region Nested type: Writer

        /// <summary>
        /// The writer.
        /// </summary>
        private class Writer : BodyWriter
        {
            /// <summary>
            /// The content.
            /// </summary>
            private readonly string content;

            /// <summary>
            /// Initializes a new instance of the <see cref="Writer"/> class.
            /// </summary>
            /// <param name="content">
            /// The content.
            /// </param>
            public Writer(string content)
                : base(false)
            {
                this.content = content;
            }

            /// <summary>
            /// The on write body contents.
            /// </summary>
            /// <param name="writer">
            /// The writer.
            /// </param>
            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("Binary");
                byte[] buffer = encoding.GetBytes(this.content);
                writer.WriteBase64(buffer, 0, buffer.Length);
                writer.WriteEndElement();
            }
        }

        #endregion
    }

    /// <summary>
    /// The jsonp support behavior attribute.
    /// Simply apply this attribute to a DataService-derived class to get
    /// JSONP support in that service
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class JSONPSupportBehaviorAttribute : Attribute, IServiceBehavior
    {
        #region IServiceBehavior Members

        /// <summary>
        /// The add binding parameters.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        /// <param name="endpoints">
        /// The endpoints.
        /// </param>
        /// <param name="bindingParameters">
        /// The binding parameters.
        /// </param>
        void IServiceBehavior.AddBindingParameters(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, 
            Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// The apply dispatch behavior.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        void IServiceBehavior.ApplyDispatchBehavior(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    ed.DispatchRuntime.MessageInspectors.Add(new JSONPSupportInspector());
                }
            }
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="serviceDescription">
        /// The service description.
        /// </param>
        /// <param name="serviceHostBase">
        /// The service host base.
        /// </param>
        void IServiceBehavior.Validate(
            ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
        }

        #endregion
    }
}