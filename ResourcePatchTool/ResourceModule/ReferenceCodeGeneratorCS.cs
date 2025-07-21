using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace ResourceModule
{
    public class ReferenceCodeGeneratorCS
    {
        public void GenerateEnumFile(List<EnumData> _enums, string _folder_path)
        {
            var provider = CodeDomProvider.CreateProvider("C#");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.VerbatimOrder = true;
            options.BlankLinesBetweenMembers = true;

            var codeCompileUnit = new CodeCompileUnit();

            var main_namespace = new CodeNamespace("ReferenceTable");
            codeCompileUnit.Namespaces.Add(main_namespace);

            var enum_types = _enums.GroupBy(e => e.type_name).ToList();

            foreach (var enum_group in enum_types)
            {
                String enum_class_name = enum_group.Key;

                CodeTypeDeclaration type = new CodeTypeDeclaration(enum_class_name);
                type.IsEnum = true;

                List<EnumData> enum_datas = enum_group.ToList();

                foreach (var enum_record in enum_group)
                {
                    CodeMemberField member_field = new CodeMemberField(enum_class_name, enum_record.data_name);
                    member_field.InitExpression = new CodePrimitiveExpression(enum_record.data_index);
                    member_field.Comments.Add(new CodeCommentStatement(enum_record.comment, false));
                    type.Members.Add(member_field);
                }
                main_namespace.Types.Add(type);
            }

            using (var string_writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(codeCompileUnit, string_writer, options);

                String class_file_path = Path.Combine(_folder_path, "RefEnum.cs");

                File.WriteAllText(class_file_path, string_writer.ToString());
            }
        }

        public void GenerateRefFile(RefDataTable _table, string _folder_path)
        {
            var code_compile_unit = new CodeCompileUnit();

            var out_side_namespace = new CodeNamespace();
            out_side_namespace.Imports.Add(new CodeNamespaceImport("System"));
            out_side_namespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            out_side_namespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            out_side_namespace.Imports.Add(new CodeNamespaceImport("System.IO"));

            code_compile_unit.Namespaces.Add(out_side_namespace);



            var main_namespace = new CodeNamespace("ReferenceTable");
            code_compile_unit.Namespaces.Add(main_namespace);

            main_namespace.Types.Add(GenerateReferenceTableClass(_table));


            var provider = CodeDomProvider.CreateProvider("C#");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.VerbatimOrder = true;
            options.BlankLinesBetweenMembers = true;

            using (var string_writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(code_compile_unit, string_writer, options);

                // 주석 제거
                String class_code_string = Regex.Replace(string_writer.ToString(), @"//(.*?)(\r?\n)+", String.Empty);
                String class_file_path = Path.Combine(_folder_path, String.Format("Ref{0}.cs", _table.table_name));

                File.WriteAllText(class_file_path, class_code_string);
            }
        }



        // 레퍼런스 테이블 클래스 생성
        private CodeTypeDeclaration GenerateReferenceTableClass(RefDataTable _table)
        {
            CodeTypeDeclaration class_type = new CodeTypeDeclaration(String.Format("Ref{0}", _table.table_name));
            class_type.IsClass = true;
            class_type.BaseTypes.Add("RefDataItem");
            class_type.IsPartial = true;


            foreach (var column in _table.columns)
            {
                if (false == Enum.IsDefined(typeof(Column.eDataType), column.data_type))
                {
                    throw new Exception(String.Format("정의되지 않은 데이터 타입입니다. {0}", column.data_type));
                }

                CodeSnippetTypeMember member = new CodeSnippetTypeMember();

                var data_type_string = column.data_type.ToString();

                switch(column.data_type)
                {
                    case Column.eDataType.INT16:
                        data_type_string = "short";
                        break;
                    case Column.eDataType.INT32:
                        data_type_string = "int";
                        break;
                    case Column.eDataType.INT64:
                        data_type_string = "long";
                        break;
                    case Column.eDataType.FLOAT:
                        data_type_string = "float";
                        break;
                    case Column.eDataType.STRING:
                        data_type_string = "string";
                        break;
                    case Column.eDataType.WSTRING:
                        data_type_string = "string";
                        break;
                    case Column.eDataType.BOOLEAN:
                        data_type_string = "bool";
                        break;
                    case Column.eDataType.DATE:
                        data_type_string = "DateTime";
                        break;
                    case Column.eDataType.ENUM:
                        data_type_string = column.enum_type_name;
                        break;
                    default:
                        throw new Exception(String.Format("정의되지 않은 데이터 타입입니다. {0}", column.data_type));
                }

                member.Text = String.Format("\t\tpublic {0} {1} {2}", data_type_string, column.data_name, "{get; set;}");

                class_type.Members.Add(member);
            }

            class_type.Members.Add(GenerateParseMethodCode(_table));
            return class_type;
        }







        /// <summary>
        /// 데이터 파싱 코드 생성
        /// </summary>
        /// <param name="refTable"></param>
        /// <returns></returns>
        private CodeMemberMethod GenerateParseMethodCode(RefDataTable _table)
        {
            String className = String.Format("Ref{0}", _table.table_name);

            var registMethod = new CodeMemberMethod();

            registMethod.Name = "Deserialize";
            registMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            //registMethod.ReturnType = new CodeTypeReference(className);
            registMethod.Parameters.Add(new CodeParameterDeclarationExpression("ByteBuffer", "br"));

            StringBuilder sb = new StringBuilder();

            String AddMethodStatement =
@"{dataparse};";

            foreach (Column column in _table.columns)
            {
                sb.Append("\t\t\t");
                switch (column.data_type)
                {
                    case Column.eDataType.INT16:
                        sb.AppendLine($"{column.data_name} = br.ReadShort();");
                        break;
                    case Column.eDataType.INT32:
                        sb.AppendLine($"{column.data_name} = br.ReadInteger();");
                        break;
                    case Column.eDataType.INT64:
                        sb.AppendLine($"{column.data_name} = br.ReadLong();");
                        break;
                    case Column.eDataType.FLOAT:
                        sb.AppendLine($"{column.data_name} = br.ReadFloat();");
                        break;
                    case Column.eDataType.STRING:
                        sb.AppendLine($"{column.data_name} = br.ReadString();");
                        break;
                    case Column.eDataType.WSTRING:
                        sb.AppendLine($"{column.data_name} = br.ReadStringUnicode();");
                        break;
                    case Column.eDataType.BOOLEAN:
                        sb.AppendLine($"{column.data_name} = br.ReadBool();");
                        break;
                    case Column.eDataType.DATE:
                        sb.AppendLine($"{column.data_name} = br.ReadDateTime();");
                        break;
                    case Column.eDataType.ENUM:
                        sb.AppendLine($"{column.data_name} = ({column.enum_type_name})br.ReadInteger();");
                        break;
                }
            }
            // KeyColumType이 Key일 경우

            AddMethodStatement = AddMethodStatement.Replace("{dataparse}", sb.ToString());
            AddMethodStatement = AddMethodStatement.Replace("{datatype}", className);

            registMethod.Statements.Add(new CodeSnippetStatement(AddMethodStatement));

            return registMethod;
        }




        public void GenerateFactoryFile(List<RefDataTable> _tables, string _folder_path)
        {
            var code_compile_unit = new CodeCompileUnit();

            var out_side_namespace = new CodeNamespace();
            out_side_namespace.Imports.Add(new CodeNamespaceImport("System"));
            out_side_namespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            out_side_namespace.Imports.Add(new CodeNamespaceImport("System.Linq"));
            out_side_namespace.Imports.Add(new CodeNamespaceImport("System.IO"));
            out_side_namespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));

            code_compile_unit.Namespaces.Add(out_side_namespace);



            var main_namespace = new CodeNamespace("ReferenceTable");
            code_compile_unit.Namespaces.Add(main_namespace);

            main_namespace.Types.Add(GenerateFactoryClass(_tables));


            var provider = CodeDomProvider.CreateProvider("C#");
            var options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.VerbatimOrder = true;
            options.BlankLinesBetweenMembers = true;

            using (var string_writer = new StringWriter())
            {
                provider.GenerateCodeFromCompileUnit(code_compile_unit, string_writer, options);

                // 주석 제거
                String class_code_string = Regex.Replace(string_writer.ToString(), @"//(.*?)(\r?\n)+", String.Empty);
                String class_file_path = Path.Combine(_folder_path, "RefFactory.cs");

                File.WriteAllText(class_file_path, class_code_string);
            }
        }


        private CodeTypeDeclaration GenerateFactoryClass(List<RefDataTable> _tables)
        {
            CodeTypeDeclaration class_type = new CodeTypeDeclaration(String.Format("RefFactory"));
            class_type.IsClass = true;
            class_type.IsPartial = true;


            class_type.Members.Add(GenerateCreateInstanceMethod());
            class_type.Members.Add(GenerateFactoryHandleMethod(_tables));
            return class_type;
        }
        private CodeMemberMethod GenerateFactoryHandleMethod(List<RefDataTable> _tables)
        {
            var create_ref_data_list_method = new CodeMemberMethod
            {
                Name = "CreateRefData",
                Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static,
                ReturnType = new CodeTypeReference("List<RefDataItem>")
            };
            // 매개변수 추가
            create_ref_data_list_method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "table_name"));
            create_ref_data_list_method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "count"));


            StringBuilder sb = new StringBuilder();

            foreach(var table in _tables)
            {
                String create_ref_data_list =
@"
		if (table_name == ""{table_name}"")
		{
			return CreateRefDataList<{table_name}>(count);
		}
";
                create_ref_data_list = create_ref_data_list.Replace("{table_name}", $"Ref{table.table_name}");
                sb.AppendLine(create_ref_data_list);
            }

            sb.AppendLine(@"
Debug.LogError($""invalid ref data table name{table_name}"");
return null;
");


            create_ref_data_list_method.Statements.Add(new CodeSnippetStatement(sb.ToString()));

            return create_ref_data_list_method;
        }

        private CodeMemberMethod GenerateCreateInstanceMethod()
        {
            var create_ref_data_list_method = new CodeMemberMethod
            {
                Name = "CreateRefDataList",
                Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static,
                ReturnType = new CodeTypeReference("List<RefDataItem>")
            };

            // 제네릭 타입 매개변수 추가
            create_ref_data_list_method.TypeParameters.Add(new CodeTypeParameter("T")
            {
                Constraints = { "RefDataItem", "new()" }
            });

            // 매개변수 추가
            create_ref_data_list_method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "count"));
            String create_ref_data_list =
@"
		var list = new List<RefDataItem>(count);
		for (int i = 0; i < count; i++)
		{
			var ref_data_item = new T();
			list.Add(ref_data_item);
		}
		return list;
";
            create_ref_data_list_method.Statements.Add(new CodeSnippetStatement(create_ref_data_list));

            return create_ref_data_list_method;
        }
    }
}
