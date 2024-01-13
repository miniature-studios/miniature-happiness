# Office architecture

Office Architecture (the name is to be changed in future) is the game about micro- and macro- office management. Hire, fire and force to work employees under the pressure of your boss tasks.

## Game design

Design document of the game can be found [here](https://docs.google.com/document/d/1oU3gORNEXA_aJ2D055r1h3WZCSSdT0YQW9pyKb6h8tQ/edit?pli=1#heading=h.ds7p1dprpmt8)[RUS]

## Code conventions

### Animators

Some animators, such as UI animators require their `updateMode` set to `AnimatorUpdateMode.UnscaledTime` but some animators such as employee animators require their `updateMode` set to `AnimatorUpdateMode.Normal` as there's ability to speed up/slow down time in game so UI animators shouldn't depend on `Time.TimeScale`.

Automatization of process of assigning correct `updateMode`s is addressed by `Utils.AnimatorTimeScaleSetterTool`. This script adds menu item `Tools -> AnimatorTimeScaleSetter` to set proper `updateMode`s to animators in both scene hierarchy and prefabs folder.

Separation into two categories is made according to the following assumptions:
- In scene hierarchy: All the UI animators contain `Canvas` in one of their parent `GameObject`s.
- In prefabs folder: All the UI animators are attached to prefabs found in `Assets\Prefabs\UI` directory and all its subdirectories.

One can easily override these rules by attaching `Utils.IgnoreAnimatorTimeScaleSetter` script to the target GameObject.

### Namespaces

It's very desirable for classes to belong to some namespace. But for now Unity doesn't support `MonoBehaviour`s that have their name different from the script's name. So the `[AddComponentMenu("...")]` attribute is used to override the component's name when script is attached to `GameObject`.

Label in attribute must have two parts:
* Namespace separated with `/` that started with `Scripts`
* Component name that contains namespace with class name
```csharp
namespace Level.Boss
{
    // Example attribute
    [AddComponentMenu("Scripts/Level/Boss/Level.Boss.Model")]
    public class Model { ... }
}
``` 

When the class with name identical to the namespace's one is needed, for example:
```csharp
namespace Level
{
    [AddComponentMenu("Level.Level")]
    public class Level { ... }
}
```

it should be renamed to `...Impl`:
```csharp
namespace Level
{
    [AddComponentMenu("Level.Level")]
    public class LevelImpl { ... }
}
```

For comfortable executing this rules, should be used [This Unity Package](https://github.com/Kiuh/Item-Templates-For-Unity) or [This Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=nikolay-khimich.unity-class-template). Both package and extension gives opportunity to create unity scripts with all rules in one click. 

### Formatting

Project uses two formatters: `.NET format` and `CSharpier`.

- Rules for `.NET format` described in [.editorconfig](https://github.com/mertwole/miniature-happiness/blob/main/JamGame/.editorconfig) file.
- Rules for `CSharpier` described in [.csharpierrc.json](https://github.com/mertwole/miniature-happiness/blob/main/JamGame/.csharpierrc.json) file.

<b>.Net format</b> it's a builtin formatter for .NET and can be found [here](https://github.com/dotnet/format).\
<b>CSharpier</b> it's a tool that's can be found [here](https://csharpier.com/) and installed for any popular IDE.

### OdinInspector

[Odin Inspector](https://odininspector.com/) used in project for comfortable debugging and proper visualization complex data structures in Inspector.

## Continuous integration
The project uses 2 ci parts, one for formatting that described in *Formatting* section.

### Unity Build Check
After any pull request for main branch, CI try to build project for Windows x32 and x64, and tells if build was failed.

### Formatting Check
Both <b>.NET format</b> and <b>CSharpier</b> checks included in CI checks and *triggers when any pull request for main branch* was created.

## License

[MIT license](https://github.com/mertwole/miniature-happiness/blob/main/LICENSE)
