# Revitapi Tutorial Book - Chinese Version
这里是《Autodesk Revit 二次开发基础教程》代码示例以及一些补充说明。
写作的过程中难免有一些疏漏之处，请大家谅解。如果有发现任何新的问题，请联系我们revitdevelopers@sina.com或者直接在github上提交一个问题，方法是：点击Issues，再点击Create an issue，然后发布该问题。

## 第一版的一些修正
第一版于2015年9月中旬出版，下面是一些需要修正的地方：
* `P5` Revit® API.dll和Revit® APIUI.dll，应该是RevitAPI.dll和RevitAPIUI.dll
* `P6` 图2-2中的Revit.ini，应该是.addin
* `P11` 代码片段2-8中，应该是<br>
```C#
  Autodesk.Revit.DB.ExternalDBApplicationResult OnShutdown(UIControlledApplication application);
  Autodesk.Revit.DB.ExternalDBApplicationResult OnStartup(UIControlledApplication application);
```
* `P19` 表2-6中，应该是<br>
```C#
  Document NewFamilyDocument
  Document NewProjectDocument
  Document NewProjectTemplateDocument
```
* `P28` Visual Studio 2010，应该是Visual Studio 2012
* `P30` C:\Program Files\Revit Architecture 2014\Program\Revit.exe，应该是C:\Program Files\Revit 2015\Revit.exe
* `P55` 图3-13，3-14中的Matg.PI，应该是Math.PI
* `P136` 表6-3描述里面的ViewSectiono, 应该是ViewSection
