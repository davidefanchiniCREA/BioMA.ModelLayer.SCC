using CRA.ModelLayer.Strategy;
using System.Collections.Generic;
using System.Linq;

namespace CRA.ModelLayer.SCC
{
    /// <summary>a switch during its building process</summary>
    public class BuildingSwitch:Switch
    {
        private string _switchname;

        public BuildingSwitch(string switchName, string switchDescription, Dictionary<string, ModellingOptions> modelingOptions,SwitchValueRelatedModelingOptions generalModelingOptions) : base( switchName,  switchDescription, modelingOptions) {

            SwitchValueRelatedModelingOptions4SwitchValues = new Dictionary<string, SwitchValueRelatedModelingOptions>();

            foreach (string switchValue in modelingOptions.Keys) {

                //by default each input (output, param, assocstrat) is associated to each value of the switch
                SwitchValueRelatedModelingOptions mo = new SwitchValueRelatedModelingOptions()
                {
                    Inputs = generalModelingOptions.Inputs.ToArray().ToList(),
                    Outputs = generalModelingOptions.Outputs.ToArray().ToList(),
                    Parameters = generalModelingOptions.Parameters.ToArray().ToList(),
                    AssociatedStrategies = generalModelingOptions.AssociatedStrategies.ToArray().ToList()
                };
                SwitchValueRelatedModelingOptions4SwitchValues.Add(switchValue,mo);
                CompositeSwitch = false;
            }
        }

        public BuildingSwitch(string switchName, string switchDescription, Dictionary<string, ModellingOptions> modelingOptions, Dictionary<string, SwitchValueRelatedModelingOptions> dictionary): base( switchName,  switchDescription, modelingOptions) 
        {            
            SwitchValueRelatedModelingOptions4SwitchValues=dictionary;          
        }

        public BuildingSwitch(string switchName, string switchDescription, string simpleStrategyName, Dictionary<string, ModellingOptions> modelingOptions)
            : base(switchName, switchDescription, modelingOptions)
        {
            CompositeSwitch = true;
            SimpleStrategyName = simpleStrategyName;
        }

        public Dictionary<string, SwitchValueRelatedModelingOptions> SwitchValueRelatedModelingOptions4SwitchValues { get; set; }

        public bool CompositeSwitch;

        public string SimpleStrategyName;
    }

    public class SwitchValueRelatedModelingOptions
    {
        public SwitchValueRelatedModelingOptions() {
            Inputs = new List<string>();
            Outputs = new List<string>();
            AssociatedStrategies = new List<string>();
            Parameters = new List<string>();
        }

        internal List<string> Inputs { get; set; }
        internal List<string> Outputs { get; set; }
        internal List<string> AssociatedStrategies { get; set; }
        internal List<string> Parameters { get; set; }
    }
}
