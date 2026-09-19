
namespace TeaSpoons.CodeModuleHelper.Editor
{
    using UnityEditor;
    using System.IO;
    using System.Text;
    using EditorToolbox.Editor;

    /// <summary>
    /// MenuItems in the Create menu to create code modules in a standardized way.
    /// </summary>
    internal class CodeModuleSetup
    {
        private const string dialogTitle = "Create Code Module";
        private const string runtimeFolder = "Runtime";
        private const string editorFolder = "Editor";
        private const string readmeFilename = "README.md";

        [MenuItem("Assets/Create/Code Module/New Code Module", priority = 1000)]
        private static void StartModuleCreation()
        {
            if (string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace))
            {
                WarnAboutMissingProjectRootNamesapce();
                return;
            }

            StringInputDialog.Show(dialogTitle, "Please enter the module name.", string.Empty, "Create", "Cancel", CreateModule);
        }

        [MenuItem("Assets/Create/Code Module/Editor Assembly", priority = 1100)]
        private static void StartCreatingEditorAssembly()
        {
            if (string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace))
            {
                WarnAboutMissingProjectRootNamesapce();
                return;
            }

            if (!ProjectPathUtility.TryGetFullActiveFolderPath(out var modulePath))
            {
                return;
            }
            if (!ModuleExists(modulePath))
            {
                EditorUtility.DisplayDialog(dialogTitle, "Could not find module path. Please create the editor assembly in a code module folder.", "Ok");
                return;
            }
            if (Directory.Exists(Path.Combine(modulePath, editorFolder)))
            {
                EditorUtility.DisplayDialog(dialogTitle, $"A folder called \"{editorFolder}\" already exists in this module.", "Ok");
                return;
            }
            CreateEditorAssembly(modulePath);

            AssetDatabase.Refresh();
        }

        private static void WarnAboutMissingProjectRootNamesapce()
        {
            EditorUtility.DisplayDialog(dialogTitle, "No root namespace set.\nYou need to set a root namespace in the project settings to create code modules.", "Ok");
            SettingsService.OpenProjectSettings("Project/Player");
        }

        /// <summary>
        /// Chacks whether the given <paramref name="modulePath"/> exists and contains a readme file,
        /// allowing for the rough assumption that the folder is a code module folder.
        /// </summary>
        private static bool ModuleExists(string modulePath)
        {
            return !string.IsNullOrEmpty(modulePath) &&
                Directory.Exists(Path.Combine(modulePath, runtimeFolder)) &&
                File.Exists(Path.Combine(modulePath, readmeFilename));
        }

        private static void CreateModule(string name)
        {
            if (!ProjectPathUtility.TryGetFullActiveFolderPath(out var parentPath))
            {
                return;
            }

            var moduleDirectory = Directory.CreateDirectory(Path.Combine(parentPath, name));

            var runtimeDirectory = Directory.CreateDirectory(Path.Combine(moduleDirectory.FullName, runtimeFolder));
            CreateAssemblyDefinition(runtimeDirectory.FullName, name);

            var createEditorAssembly = EditorUtility.DisplayDialog(dialogTitle, "Generate an editor assembly?", "Yes", "No");
            if (createEditorAssembly)
            {
                CreateEditorAssembly(moduleDirectory.FullName);
            }

            CreateReadme(moduleDirectory.FullName, name);

            AssetDatabase.Refresh();
        }

        private static void CreateEditorAssembly(string modulePath)
        {
            var editorDirectory = Directory.CreateDirectory(Path.Combine(modulePath, editorFolder));
            var runtimePath = Path.Combine(modulePath, runtimeFolder);
            var moduleName = new DirectoryInfo(modulePath).Name;

            CreateAssemblyDefinition(editorDirectory.FullName, moduleName, true);
            CreateAssemblyInfo(runtimePath, moduleName);
        }

        private static void CreateAssemblyDefinition(string path, string assemblyName, bool isEditorAssembly = false)
        {
            assemblyName = GetNamespace(assemblyName);
            var actualAssemblyName = assemblyName;
            if (isEditorAssembly)
            {
                actualAssemblyName += ".Editor";
            }
            var fullAssemblyName = $"{EditorSettings.projectGenerationRootNamespace}.{actualAssemblyName}";

            var content = new StringBuilder();

            content.AppendLine("{");
            content.AppendLine($"    \"name\": \"{fullAssemblyName}\",");

            content.AppendLine($"    \"rootNamespace\": \"{fullAssemblyName}\",");

            content.AppendLine("    \"references\": [");
            if (isEditorAssembly)
            {
                content.AppendLine($"        \"{EditorSettings.projectGenerationRootNamespace}.{assemblyName}\"");
            }
            content.AppendLine("    ],");

            content.AppendLine("    \"optionalUnityReferences\": [],");

            if (isEditorAssembly)
            {
                content.AppendLine("    \"includePlatforms\": [");
                content.AppendLine("        \"Editor\"");
                content.AppendLine("    ],");
            }
            else
            {
                content.AppendLine("    \"includePlatforms\": [],");
            }
            content.AppendLine("    \"excludePlatforms\": [],");

            content.AppendLine("    \"allowUnsafeCode\": false,");
            content.AppendLine("    \"overrideReferences\": false,");
            content.AppendLine("    \"autoReferenced\": true,");
            content.AppendLine("    \"defineConstraints\": []");
            content.AppendLine("}");

            var filePath = Path.Combine(path, fullAssemblyName + ".asmdef");
            File.WriteAllText(filePath, content.ToString());
        }

        /// <summary>
        /// Returns the given string formatted as a namespace.
        /// </summary>
        /// <example>
        /// "Hello-World" -> "HelloWorld"
        /// "hello world" -> "HelloWorld"
        /// </example>
        private static string GetNamespace(string assemblyName)
        {
            assemblyName = assemblyName.Trim();
            var result = new StringBuilder();

            var wasSpace = false;
            foreach (var character in assemblyName)
            {
                if (!char.IsLetterOrDigit(character))
                {
                    wasSpace = true;
                    continue;
                }

                if (char.IsDigit(character))
                {
                    result.Append(character);
                    wasSpace = true;
                    continue;
                }

                if (result.Length == 0 || wasSpace)
                {
                    result.Append(char.ToUpper(character));
                }
                else
                {
                    result.Append(character);
                }

                wasSpace = false;
            }

            return result.ToString();
        }

        private static void CreateAssemblyInfo(string path, string assemblyName)
        {
            assemblyName = GetNamespace(assemblyName);
            var fullAssemblyName = $"{EditorSettings.projectGenerationRootNamespace}.{assemblyName}";

            var content = new StringBuilder();

            content.AppendLine();
            content.AppendLine($"[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(\"{fullAssemblyName}.Editor\")]");

            var filePath = Path.Combine(path, "AssemblyInfo.cs");
            File.WriteAllText(filePath, content.ToString());
        }

        private static void CreateReadme(string path, string name)
        {
            var content = new StringBuilder();
            content.AppendLine($"# {name}");
            content.AppendLine("Write a summary of what this module does here.");
            content.AppendLine();
            content.AppendLine("## Setup");
            content.AppendLine("Detail initial steps required for usage below.");
            content.AppendLine("Feel free to remove this section if there is no required setup.");
            content.AppendLine();
            content.AppendLine("## Usage");
            content.AppendLine("Document usage of this module here.");
			
            var filePath = Path.Combine(path, readmeFilename);
            File.WriteAllText(filePath, content.ToString());
        }
    }
}
