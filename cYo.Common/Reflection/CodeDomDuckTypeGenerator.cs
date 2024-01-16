// Decompiled with JetBrains decompiler
// Type: cYo.Common.Reflection.CodeDomDuckTypeGenerator
// Assembly: cYo.Common, Version=1.0.5915.38773, Culture=neutral, PublicKeyToken=b3ca110c99b4b731
// MVID: 5D64BD1B-C14B-43A5-BB76-3BCC544EA860
// Assembly location: C:\Program Files\ComicRack\cYo.Common.dll

using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

#nullable disable
namespace cYo.Common.Reflection
{
  internal class CodeDomDuckTypeGenerator : IDuckTypeGenerator
  {
    private const string TypePrefix = "Duck";
    private const string CommonNamespace = "DynamicDucks";

    public Type[] CreateDuckTypes(Type interfaceType, Type[] duckedTypes)
    {
      string name = "DynamicDucks." + interfaceType.Name;
      CodeCompileUnit compileUnit = new CodeCompileUnit();
      CodeNamespace codeNamespace = new CodeNamespace(name);
      compileUnit.Namespaces.Add(codeNamespace);
      CodeTypeReference codeTypeReference = new CodeTypeReference(interfaceType);
      CodeDomDuckTypeGenerator.ReferenceList referenceList = new CodeDomDuckTypeGenerator.ReferenceList();
      for (int index = 0; index < duckedTypes.Length; ++index)
      {
        Type duckedType = duckedTypes[index];
        CodeTypeReference type = new CodeTypeReference(duckedType);
        referenceList.AddReference(duckedType);
        CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration("Duck" + (object) index);
        codeNamespace.Types.Add(codeTypeDeclaration);
        codeTypeDeclaration.TypeAttributes = TypeAttributes.Public;
        codeTypeDeclaration.BaseTypes.Add(codeTypeReference);
        CodeMemberField codeMemberField = new CodeMemberField(type, "_obj");
        codeTypeDeclaration.Members.Add((CodeTypeMember) codeMemberField);
        CodeFieldReferenceExpression referenceExpression1 = new CodeFieldReferenceExpression((CodeExpression) new CodeThisReferenceExpression(), codeMemberField.Name);
        CodeConstructor codeConstructor = new CodeConstructor();
        codeTypeDeclaration.Members.Add((CodeTypeMember) codeConstructor);
        codeConstructor.Attributes = MemberAttributes.Public;
        codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(type, "obj"));
        codeConstructor.Statements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) referenceExpression1, (CodeExpression) new CodeArgumentReferenceExpression("obj")));
        foreach (MethodInfo method in interfaceType.GetMethods())
        {
          if ((method.Attributes & MethodAttributes.SpecialName) == MethodAttributes.PrivateScope)
          {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
            codeTypeDeclaration.Members.Add((CodeTypeMember) codeMemberMethod);
            codeMemberMethod.Name = method.Name;
            codeMemberMethod.ReturnType = new CodeTypeReference(method.ReturnType);
            codeMemberMethod.PrivateImplementationType = codeTypeReference;
            referenceList.AddReference(method.ReturnType);
            ParameterInfo[] parameters = method.GetParameters();
            CodeArgumentReferenceExpression[] referenceExpressionArray = new CodeArgumentReferenceExpression[parameters.Length];
            int num = 0;
            foreach (ParameterInfo parameterInfo in parameters)
            {
              referenceList.AddReference(parameterInfo.ParameterType);
              CodeParameterDeclarationExpression declarationExpression = new CodeParameterDeclarationExpression(parameterInfo.ParameterType, parameterInfo.Name);
              codeMemberMethod.Parameters.Add(declarationExpression);
              referenceExpressionArray[num++] = new CodeArgumentReferenceExpression(parameterInfo.Name);
            }
            CodeMethodInvokeExpression expression = new CodeMethodInvokeExpression((CodeExpression) referenceExpression1, method.Name, (CodeExpression[]) referenceExpressionArray);
            if (method.ReturnType == typeof (void))
              codeMemberMethod.Statements.Add((CodeExpression) expression);
            else
              codeMemberMethod.Statements.Add((CodeStatement) new CodeMethodReturnStatement((CodeExpression) expression));
          }
        }
        foreach (PropertyInfo property in interfaceType.GetProperties())
        {
          CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
          codeTypeDeclaration.Members.Add((CodeTypeMember) codeMemberProperty);
          codeMemberProperty.Name = property.Name;
          codeMemberProperty.Type = new CodeTypeReference(property.PropertyType);
          codeMemberProperty.Attributes = MemberAttributes.Public;
          codeMemberProperty.PrivateImplementationType = new CodeTypeReference(interfaceType);
          referenceList.AddReference(property.PropertyType);
          ParameterInfo[] indexParameters = property.GetIndexParameters();
          CodeArgumentReferenceExpression[] referenceExpressionArray = new CodeArgumentReferenceExpression[indexParameters.Length];
          int num = 0;
          foreach (ParameterInfo parameterInfo in indexParameters)
          {
            CodeParameterDeclarationExpression declarationExpression = new CodeParameterDeclarationExpression(parameterInfo.ParameterType, parameterInfo.Name);
            codeMemberProperty.Parameters.Add(declarationExpression);
            referenceList.AddReference(parameterInfo.ParameterType);
            CodeArgumentReferenceExpression referenceExpression2 = new CodeArgumentReferenceExpression(parameterInfo.Name);
            referenceExpressionArray[num++] = referenceExpression2;
          }
          if (property.CanRead)
          {
            codeMemberProperty.HasGet = true;
            if (referenceExpressionArray.Length == 0)
              codeMemberProperty.GetStatements.Add((CodeStatement) new CodeMethodReturnStatement((CodeExpression) new CodePropertyReferenceExpression((CodeExpression) referenceExpression1, property.Name)));
            else
              codeMemberProperty.GetStatements.Add((CodeStatement) new CodeMethodReturnStatement((CodeExpression) new CodeIndexerExpression((CodeExpression) referenceExpression1, (CodeExpression[]) referenceExpressionArray)));
          }
          if (property.CanWrite)
          {
            codeMemberProperty.HasSet = true;
            if (referenceExpressionArray.Length == 0)
              codeMemberProperty.SetStatements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodePropertyReferenceExpression((CodeExpression) referenceExpression1, property.Name), (CodeExpression) new CodePropertySetValueReferenceExpression()));
            else
              codeMemberProperty.SetStatements.Add((CodeStatement) new CodeAssignStatement((CodeExpression) new CodeIndexerExpression((CodeExpression) referenceExpression1, (CodeExpression[]) referenceExpressionArray), (CodeExpression) new CodePropertySetValueReferenceExpression()));
          }
        }
        foreach (EventInfo eventInfo in interfaceType.GetEvents())
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("public event " + eventInfo.EventHandlerType.FullName + " @" + eventInfo.Name + "{");
          stringBuilder.Append("add    {" + codeMemberField.Name + "." + eventInfo.Name + "+=value;}");
          stringBuilder.Append("remove {" + codeMemberField.Name + "." + eventInfo.Name + "-=value;}");
          stringBuilder.Append("}");
          referenceList.AddReference(eventInfo.EventHandlerType);
          codeTypeDeclaration.Members.Add((CodeTypeMember) new CodeSnippetTypeMember(stringBuilder.ToString()));
        }
      }
      CSharpCodeProvider csharpCodeProvider = new CSharpCodeProvider();
      StringWriter writer = new StringWriter();
      csharpCodeProvider.GenerateCodeFromCompileUnit(compileUnit, (TextWriter) writer, new CodeGeneratorOptions());
      Console.WriteLine(writer.ToString());
      CompilerParameters compilerParameters = new CompilerParameters();
      compilerParameters.GenerateInMemory = true;
      compilerParameters.ReferencedAssemblies.Add(interfaceType.Assembly.Location);
      referenceList.SetToCompilerParameters(compilerParameters);
      CompilerResults compilerResults = csharpCodeProvider.CompileAssemblyFromDom(compilerParameters, compileUnit);
      if (compilerResults.Errors.Count > 0)
      {
        StringWriter stringWriter = new StringWriter();
        foreach (CompilerError error in (CollectionBase) compilerResults.Errors)
          stringWriter.WriteLine(error.ErrorText);
        throw new Exception("Compiler-Errors: \n\n" + (object) stringWriter);
      }
      Assembly compiledAssembly = compilerResults.CompiledAssembly;
      Type[] duckTypes = new Type[duckedTypes.Length];
      for (int index = 0; index < duckedTypes.Length; ++index)
        duckTypes[index] = compiledAssembly.GetType(name + ".Duck" + (object) index);
      return duckTypes;
    }

    private class ReferenceList
    {
      private readonly List<string> list = new List<string>();
      private static readonly Assembly mscorlib = typeof (object).Assembly;

      public bool AddReference(Assembly assembly)
      {
        if (this.list.Contains(assembly.Location) || !(assembly != CodeDomDuckTypeGenerator.ReferenceList.mscorlib))
          return false;
        this.list.Add(assembly.Location);
        return true;
      }

      public void AddReference(Type type)
      {
        this.AddReference(type.Assembly);
        if (!(type.BaseType.Assembly != CodeDomDuckTypeGenerator.ReferenceList.mscorlib))
          return;
        this.AddReference(type.BaseType);
      }

      public void SetToCompilerParameters(CompilerParameters parameters)
      {
        foreach (string str in this.list)
          parameters.ReferencedAssemblies.Add(str);
      }
    }
  }
}
