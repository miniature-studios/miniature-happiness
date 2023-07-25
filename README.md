# Office architecture

Office Architecture (the name is to be chaged in future) is the game abount micro- and macro- office management.

## Game desing

Desing document of the game can be found [here](https://docs.google.com/document/d/1oU3gORNEXA_aJ2D055r1h3WZCSSdT0YQW9pyKb6h8tQ/edit?pli=1#heading=h.ds7p1dprpmt8)[RUS]

## Code conventions

#### Codegen

The [codegen](https://github.com/AnnulusGames/UnityCodeGen) is used in the project to add ability to make interface types visible and editable in editor. This behaviour is achieved by adding `[InterfaceEditor]` attribute to the target interface and then using [codegen](https://github.com/AnnulusGames/UnityCodeGen) to generate additional files that are placed in the `Generated` folder.

#### Animators

Some animators, such as UI animators require their `updateMode` set to `AnimatorUpdateMode.UnscaledTime` but some animators such as employee animators reqire their `updateMode` set to `AnimatorUpdateMode.Normal` as there's ability to speed up/slow down time in game so UI animators shouldn't depend on `Time.TimeScale`.

Automatization of process of assigning correct `updateMode`s is addressed by `Utils.AnimatorTimeScaleSetterTool`. This script adds menu item `Tools -> AnimatorTimeScaleSetter` to set proper `updateMode`s to animators in both scene hierarchy and prefabs folder.

Separation into two categories is made according to the following assumptions:
- In scene hierarchy: All the UI animators contain `Canvas` in one of their parent `GameObject`s.
- In prefabs folder: All the UI animators are attached to prefabs found in `Assets\Prefabs\UI` directory and all its subdirectories.

One can easily override these rules by attaching `Utils.IgnoreAnimatorTimeScaleSetter` script to the target GameObject.

#### Namespaces

It's very desirable for classes to belong to some namespace. But for now Unity doesn't support `MonoBehaviour`s that have their name different from the script's name. So the `[AddComponentMenu("NAMESPACE.CLASS_NAME")]` attribute is used to override the component's name when script is attached to `GameObject`.

When the class with name identical to the namespace's one is needed, for example:
```
namespace Level
{
[AddComponentMenu("Level.Level")]
public class Level { }
}
```

it should be renamed to `...Impl`:
```
namespace Level
{
[AddComponentMenu("Level.Level")]
public class LevelImpl { }
}
```

#### Formatting

The two code formatters are used in the project:
- [CSharpier](https://csharpier.com/) is used to control the identations, line length, empty lines, e.t.c.
- [Visual Studio's Code Style configuration](https://learn.microsoft.com/en-us/visualstudio/ide/code-styles-and-code-cleanup?view=vs-2022) is mainly used to control naming style and also carries other preferences like preferring `readonly` fields, preferring `switch` expressions and so on. The full configuration can be found at [.editorconfig](https://github.com/mertwole/miniature-happiness/blob/main/JamGame/.editorconfig).

### License

[MIT license](https://github.com/mertwole/miniature-happiness/blob/main/LICENSE)
