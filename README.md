# CodeModule Helper
An editor tool for managing code modules in a consistent, standardized way.

## Code Module Structure
A code module is basically a _custom package_ [as defined by Unity](https://docs.unity3d.com/Manual/cus-layout.html),
but reduced by some parts that aren't needed when embedded in a project, like the package manifest.

This leaves us with the following folder structure:
- CodeModule Root Folder/
  - Runtime/
    - Runtime Assembly Definition
    - `AssemblyInfo.cs`
  - Editor/
    - Editor Assembly Definition

The following rules apply:
- The root folder will have the name of the code module.
- Both **assemblies are optional**, but the tool will generate the runtime assembly folder by default.
- The **runtime assembly definition** and **the namespace** of that assembly will be named `<ProjectRootNameSpace>.<CodeModuleName>`.
- The **editor assembly definition** will append `.Editor` to both.
- The `AssemblyInfo.cs` in the runtime assembly folder contains an `InternalsVisibleTo` attribute that makes the internals of the runtime assembly visible to the editor assembly if it exists.

## Helper Workflow
To use the helper, make sure that the **Project Root Namespace** is set to `TeaSpoons.<ProjectName>` in the project settings.

### Creating a new code module
1. Open the folder you want to put it into in the project view.
2. Open the create menu (right-click/create) and scroll down to the `Code Module` item.
3. Select `New Code Module`.
4. Enter the name of the code module. Feel free to use spaces.
5. In the dialog, decide on whether an editor assembly folder should be generated.
If you select no, you can create one later using the `Editor Assembly` item next to the `Code Module` item in the create menu.

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/code-module-helper.git
```

Pin a release by appending a tag, for example `#v0.2.2`.

### Dependencies

Unity cannot resolve git dependencies automatically, so add these to your project first:

- `com.tea-spoons.package-core` 1.4.0
- `com.tea-spoons.editor-toolbox` 0.4.0

## Change plan

See [CHANGE-PLAN.md](CHANGE-PLAN.md) for what changed before publishing and what is planned next.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
