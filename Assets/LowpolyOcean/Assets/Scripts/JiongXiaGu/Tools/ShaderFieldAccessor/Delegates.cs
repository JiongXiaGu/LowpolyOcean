using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.ShaderTools
{

    public delegate bool ShaderFieldAction(IShaderFieldGroup field);
    public delegate bool ShaderFieldAction1(IShaderFieldGroup field, object ins);
    public delegate bool ShaderFieldAction2(IShaderFieldGroup field, object ins0, object ins1);
    public delegate bool ShaderFieldAction3(IShaderFieldGroup field, object ins0, object ins1, object ins2);

}
