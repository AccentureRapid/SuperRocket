using HibernatingRhinos.Profiler.Appender.NHibernate;
using Orchard.Environment;

namespace SoNerdy.NHProf.Events {
    public class OrchardShellEvents : IOrchardShellEvents{
        public void Activated()
        {
            #if DEBUG
                NHibernateProfiler.Initialize();
            #endif
        }

        public void Terminating() {
            
        }
    }
}