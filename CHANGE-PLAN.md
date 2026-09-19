# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.code-module-helper` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).
- Requires `editor-toolbox` 0.4.0, which has a new `StringInputDialog` written from scratch (the original one came from a gist without a stated license).

## Planned changes

- [x] Tag and publish `v0.2.2` with the Release workflow.
- [ ] Make installs resolve dependencies automatically, for example through a registry such as OpenUPM.
<!-- review-items:start -->
- [ ] **P0** Check `monoScript != null` before calling `GetClass()`, and add a test for a missing script.
- [ ] **P1** Look up scripts only for the types returned by `TypeCache` instead of loading every `MonoScript` in the project; measure in a project with several thousand scripts.
- [ ] **P1** Add editor tests: attribute discovery, `CodeStatus` mapping, and the empty state.
- [ ] **P1** Declares `unity: 2022.3`, but only Unity 6000.3.8f1 was tested. Add a Unity version matrix to CI once package tests run there (see the `unity-ci-kit` plan), or raise the minimum.
- [ ] **P2** Optional Roslyn analyzer that reports `[Unfinished]` types as warnings, so they also show in CI logs. It connects to the removed logging analyzer item in the `logging` plan.
- [ ] **P2** Make the package-core dependency optional (menu root only).
- [ ] **P2** Add a `CHANGELOG.md`. Unity's package layout lists one next to `README.md`, and the `unity-ci-kit` validator warns without it.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [Unity manual: Roslyn analyzers and source generators](https://docs.unity3d.com/2021.2/Documentation/Manual/roslyn-analyzers.html) | Unity documentation | Unity treats assets with the `RoslynAnalyzer` label as analyzers or source generators, so a convention such as "this type is unfinished" can become a compiler warning instead of a window. |

### Findings from reading the code

- **[Bug]** `UnfinishedCodeWindow.OnEnable` calls `monoScript.GetClass()` before it checks `monoScript != null`. A script deleted between `FindAssets` and `LoadAssetAtPath` gives a `NullReferenceException`.
- **[Perf]** It loads every `MonoScript` in the project (`FindAssets("t:MonoScript")`) each time the window is enabled, including after every domain reload, although `TypeCache.GetTypesWithAttribute` already returns the few tagged types.
- **[Coupling]** It depends on package-core and editor-toolbox; package-core is only used for the menu root.
- **[Tests]** None.
<!-- review:end -->

## Notes and ideas

_Add your own here._
