#define DEBUG_PRINT


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace AdvancedStep
{
    class Attribute
    {
        /*
            Conditional attribute
            
            - for debugging
            - return void method
            - not use override method
            - not use member method of interface
            - use System.Diagnostics
            - preprocessor
        */
        [Conditional("DEBUG_PRINT")]
        public static void DebugPrint(string debugMessage)
        {
            Console.WriteLine(debugMessage);
        }

        static void Conditional_attribute()
        {
            DebugPrint("debug message by conidtional !!!");
        }


        /*
            DllImport attribute

            - for use external DLL
            - use System.Runtime.InteropServices
        */
        [DllImport("User32.Dll")]
        public static extern int MessageBox(int i, string text, string title, int type);

        static void DllImport_attribute()
        {
            MessageBox(0, "MessageBox Test", "DllImport Test", 2);
        }


        /*
            Obsolete attribute

            - for use deprecated method
        */
        [Obsolete("Deprecated MethodOld(), use MethodNew() instead")]
        public static void MethodOld()
        {
            Console.WriteLine("call MethodOld()");
        }

        public static void MethodNew()
        {
            Console.WriteLine("call MethodNew()");
        }

        static void Obsolete_attribute()
        {
            MethodOld(); //'Attribute.MethodOld()'은(는) 사용되지 않습니다. 'Deprecated MethodOld(), use MethodNew() instead'
            MethodNew();
        }


        /*
            Custom attribute    
        */
        [AttributeUsage( AttributeTargets.Class
                       | AttributeTargets.Constructor
                       | AttributeTargets.Field
                       | AttributeTargets.Method
                       | AttributeTargets.Property
                       , AllowMultiple = true )]
        public class CustomAttribute : System.Attribute
        {
            private string text;

            public CustomAttribute(string text)
            {
                this.Text = text;
            }

            public string Text { get; set; }
        }

        [AttributeUsage(AttributeTargets.Property)]
        class CustomCheckAttribute : System.Attribute
        {
            public int MaxLength { get; set; }
        }

        [CustomAttribute("Hello World...")]
        public class SomeClass
        {
            public void Display()
            {
                MemberInfo memberInfo = typeof(SomeClass);
                object[] attributes = memberInfo.GetCustomAttributes(true);

                foreach (object attribute in attributes)
                {
                    CustomAttribute customAttribute = attribute as CustomAttribute;

                    if (customAttribute != null)
                    {
                        Console.WriteLine("Text = {0}", customAttribute.Text);
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }

            [CustomCheck(MaxLength = 10)]
            public string SomeCode { get; set; }

            public void DisplaySomeCode()
            {
                this.SomeCode = "123456789";

                Type objType = this.GetType();

                foreach (PropertyInfo p in objType.GetProperties())
                {
                    //for every property loop through all attributes
                    foreach (var attrib in p.GetCustomAttributes(false))
                    {
                        CustomCheckAttribute checker = attrib as CustomCheckAttribute;

                        if (p.Name == "SomeCode")
                        {
                            //do the length check and and raise exception accordingly
                            if (this.SomeCode.Length > checker.MaxLength)
                            {
                                throw new Exception("Max length issues");
                            }
                        }

                        Console.WriteLine("SomeCodeLength = {0}", SomeCode.Length);
                    }
                }
            }
        }

        static void custom_attribute()
        {
            var someObj = new SomeClass();
            someObj.Display();
            someObj.DisplaySomeCode();
        }


        public static void Test()
        {
            //custom_attribute();

            //Obsolete_attribute();

            //DllImport_attribute();

            //Conditional_attribute();
        }
    }
}
