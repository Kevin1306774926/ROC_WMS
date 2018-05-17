using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ROC.Comm
{
    public class ScaffolderExtends
    {
        public static string TestMathord()
        {
            return "Override the Default Scaffold Templates,czp!";
        }
        public static string GetDisplayNameFromAtrribute(string viewDataTypeName, string propertyName)
        {
            string ret = string.Empty;
            //如果当string表示的目标类型不在当前程序集中，则运行时Type.GetType会返回null
            //因为Type.GetType只会在当前程序集中进行类型搜索！
            //解决的办法1：首先加载目标程序集，然后再使用Assembly.GetType方法来获取类型            
            //比如：Assembly asmb = Assembly.LoadFrom("MVCScaffolderTest.dll");
            //        Type typeModel = asmb.GetType(viewDataTypeName);

            //解决的办法2: 将方法Type.GetType() 放在同一个程序集中
            Type typeModel = Type.GetType(viewDataTypeName);
            if (typeModel != null)
            {
                var p = typeModel.GetProperty(propertyName);
                var displaynameAtrribute = p.GetCustomAttribute<DisplayAttribute>();
                //Attribute displaynameAtrribute = (DisplayAttribute)Attribute.GetCustomAttribute(typeModel.GetProperty(propertyName), typeof(DisplayAttribute));
                if (displaynameAtrribute != null)
                {
                    ret = displaynameAtrribute.Name;
                    //ret = ((DisplayAttribute)displaynameAtrribute).Name;
                }
            }
            return ret;
        }
    }
}
