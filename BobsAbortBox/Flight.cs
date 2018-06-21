using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BobsPanicBox
{
    internal class Flight
    {
        internal AbortValues av;

        internal Flight()
        {
            av = new AbortValues(this);
        }
        public void SaveCurrent(Module_BobsPanicBox m)
        {
            Log.Info("Flight.SaveCurrent");
            av.SaveCurrent(m);
        }
        public bool Changed(Module_BobsPanicBox m)
        {
            return av.Changed(m);
        }

        public void SetAllValues(Vessel vessel, AbortValues a)
        {
            av = a;
            foreach (var p in vessel.parts)
            {
                var m = p.FindModuleImplementing<Module_BobsPanicBox>();
                if (m != null)
                {
                    m.SetAllValues(av);
                }
            }
        }
    }
}
