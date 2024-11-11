// v2.9 - BE2_Attributes script added to hold needed BE2 attributes 

namespace MG_BlocksEngine2.Attribute
{
    // v2.9 - [SerializeAsVariable] attribute used on a BE2 Instruction thta indicates if this Block must be serialized as a variable, 
    // being dinamically recreated on the variables manager and the blocks selection panel
    public class SerializeAsVariableAttribute : System.Attribute
    {
        public System.Type variablesManagerType;

        public SerializeAsVariableAttribute(System.Type variablesManagerType)
        {
            this.variablesManagerType = variablesManagerType;
        }
    }
}