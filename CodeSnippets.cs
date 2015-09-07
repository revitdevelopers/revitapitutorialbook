//============代码片段2-1：外部命令中Excute函数的定义============
public interface IExternalCommand
{
  public Autodesk.Revit.UI.Result Execute(
   Autodesk.Revit.UI.ExternalCommandData commandData,
   ref string message,
   Autodesk.Revit.DB.ElementSet elements)
}

//============代码片段2-2：从commandData中取到Document============
UIApplication uiApplication = commandData.Application;
Application application = uiApplication.Application;
UIDocument uiDocument = uiApplication.ActiveUIDocument;
Document document = uiDocument.Document;

//============代码片段2-3：使用message参数============
   public class command : IExternalCommand
   {
      public Result Execute(
             ExternalCommandData commandData,
             ref string message,
             ElementSet elements)
      {
         message = "message test";
         return Result.Failed;
      }
   }

//============代码片段2-4：使用element参数============
public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
{
   message = "Please take attention on the highlighted Walls!";
   //先从UI选取元素，然后执行该插件
   ElementSet elems = commandData.Application.ActiveUIDocument.Selection.Elements;
   foreach (Element elem in elems)
   {
      Wall wall = elem as Wall;
      if (null != wall)
      {
   elements.Insert(elem);
      }
   }
   return Result.Failed;
}

//============代码片段2-5：外部命令中Excute函数的返回值============
public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
{
   try
   {
      UIDocument uiDoc = commandData.Application.ActiveUIDocument;
      Document doc = uiDoc.Document;
      List<ElementId> selectedElem = new List<ElementId>();
      foreach(Element elem in uiDoc.Selection.Elements)
      {
         selectedElem.Add(elem.Id);
      }

      doc.Delete(selectedElem);

      TaskDialogResult result = TaskDialog.Show(
         "Revit",
         "Yes to return succeeded and delete all selection,"+
         "No to cancel all commands.",
         TaskDialogCommonButtons.Yes|TaskDialogCommonButtons.No);

      if (TaskDialogResult.Yes == result)
      {
         return Result.Succeeded;
      }
      else if (TaskDialogResult.No == result)
      {
         elements = uiDoc.Selection.Elements;
         message = "Failed to delete selection.";
         return Result.Failed;
      }
      else
      {
         return Result.Cancelled;
      }
   }
   catch
   {
      message = "Unexpected Exception is thrown out.";
      return Result.Failed;
   }
}

//============代码片段2-6：IExternalApplication接口定义============
public interface IExternalApplication
{
   Autodesk.Revit.UI.Result OnShutdown(UIControlledApplication application);
   Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application);
}

//============代码片段2-7：使用IExternalApplication定制UI============
public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
{
   //添加一个新的Ribbon面板
   RibbonPanel ribbonPanel = application.CreateRibbonPanel("NewRibbonPanel");

   //在新的Ribbon面板上添加一个按钮
   //点击这个按钮，调用本章第四节第一个实例。
   PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("HelloRevit",
       "HelloRevit", @"C:\Projects\HelloRevit\HelloRevit.dll", "HelloRevit.Class1")) as PushButton;
   return Result.Succeeded;
}

public Result OnShutdown(UIControlledApplication application)
{
   //UI定制不需要特别在OnShutdown方法中做处理。
   return Result.Succeeded;
}

//============代码片段2-8：IExternalDBApplication接口定义============
public interface IExternalDBApplication
{
  Autodesk.Revit.DB.ExternalDBApplicationResult OnShutdown(UIControlledApplication application);
  Autodesk.Revit.DB.ExternalDBApplicationResult OnStartup(UIControlledApplication application);
}

//============代码片段2-9：ExternalCommand的.addin文件格式示例============
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Assembly>c:\MyProgram\MyProgram.dll</Assembly>
    <AddInId>76eb700a-2c85-4888-a78d-31429ecae9ed</AddInId>
    <FullClassName>Revit.Samples.SampleCommand</FullClassName>
    <Text>Sample command</Text>
    <VisibilityMode>NotVisibleInFamily</VisibilityMode>
    <VisibilityMode>NotVisibleInMEP</VisibilityMode>
    <AvailabilityClassName>Revit.Samples.SampleAccessibilityCheck</AvailabilityClassName>
    <LongDescription><p>This is the long description for my command.</p><p>This is another descriptive paragraph, with notes about how to use the command properly.</p></LongDescription>
    <TooltipImage>c:\MyProgram\Autodesk.jpg</TooltipImage>
    <LargeImage>c:\MyProgram\MyProgramIcon.png</LargeImage>
    <VendorId>ADSK</VendorId>
    <VendorDescription>Autodesk, www.autodesk.com</VendorDescription>
  </AddIn>
</RevitAddIns>

//============代码片段2-10：ExternalApplication的.addin文件格式示例============
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>SampleApplication</Name>
    <Assembly>c:\MyProgram\MyProgram.dll</Assembly>
    <AddInId>604B1052-F742-4951-8576-C261D1993107</AddInId>
    <FullClassName>Revit.Samples.SampleApplication</FullClassName>
    <VendorId>ADSK</VendorId>
    <VendorDescription>Autodesk, www.autodesk.com</VendorDescription>
  </AddIn>
</RevitAddIns>

//============代码片段2-11：数据库级别ExternalApplication的.addin文件格式示例============
<?xml version="1.0" standalone="no"?>
<RevitAddIns>
   <AddIn Type="DBApplication">
      <Assembly>c:\MyDBLevelApplication\MyDBLevelApplication.dll</Assembly>
      <AddInId>DA3D570A-1AB3-4a4b-B09F-8C15DFEC6BF0</AddInId>
      <FullClassName>MyCompany.MyDBLevelAddIn</FullClassName>
      <Name>My DB-Level AddIn</Name>
      <VendorId>ADSK</VendorId>
      <VendorDescription>Autodesk, www.autodesk.com</VendorDescription>
   </AddIn>
</RevitAddIns>

//============代码片段2-12：外部命令中Excute函数的Transaction属性============
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic)]
public class Class1 : IExternalCommand
{
}

//============代码片段2-13：外部命令中Excute函数的Journaling属性============
[Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
public class Class1 : IExternalCommand
{
}

//============代码片段2-14：获取Application对象============
Autodesk.Revit.ApplicationServices.Application app
         = commandData.Application.Application;

//============代码片段2-15：Revit版本及产品信息============
public void GetVersionInfo(Autodesk.Revit.ApplicationServices.Application app)
{
   if (app.VersionNumber == "2014")
   {
      TaskDialog.Show("Supported version",
                      "This application supported in this version.");
   }
   else
   {
      TaskDialog dialog = new TaskDialog("Unsupported version.");
      dialog.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
      dialog.MainInstruction = "This application is only supported in Revit 2014.";
      dialog.Show();
   }
}

//============代码片段2-16：获取UIApplication对象============
Autodesk.Revit.UI.UIpplication uiApp = commandData.Application;

//============代码片段2-17：获取文档对象============
Autodesk.Revit.UI.UIDocument activeDoc = commandData.Application.ActiveUIDocument;
Autodesk.Revit.DB.DocumentSet documents = commandData.Application.Application.Documents;

//============代码片段2-18：从Setting中取到当前文档的Categories============
// 从当前文档对象中取到Setting对象
Settings documentSettings = document.Settings;
String prompt = "Number of all categories in current Revit document: " + documentSettings.Size+"\n";

// 用BuiltInCategory枚举值取到一个对应的Floor Category，打印其名字
Category floorCategory = documentSettings.get_Item(BuiltInCategory.OST_Floors);
prompt +="Get floor category and show the name: ";
prompt += floorCategory.Name;

TaskDialog.Show("Revit", prompt);

//============代码片段2-19：使用Category ID============
Element selectedElement = null;
foreach (Element e in document.Selection.Elements)
{
        selectedElement = e;
        break;
}
// 取到当前元素的类别
Category category = selectedElement.Category;
BuiltInCategory enumCategory = (BuiltInCategory)category.Id.Value;

//============代码片段2-20：打印当前文档中的可打印视图============
public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
{
  Document doc = commandData.Application.ActiveUIDocument.Document;
  FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan));
  IList<Element> viewElems = collector.ToElements();
  ViewSet printableViews = new ViewSet();

  // 找出全部可打印视图
  foreach (View view in viewElems)
  {
     if (!view.IsTemplate && view.CanBePrinted)
     {
        printableViews.Insert(view);
     }
  }
  PrintManager pm = doc.PrintManager;
  pm.PrintRange = PrintRange.Select;
  pm.SelectNewPrintDriver(@"\\server\printer01");
  pm.Apply();

  // 打印全部可打印视图
  doc.Print(printableViews);
  return IAsyncResult.Succeeded;
}

//============代码片段2-21：使用事务创建元素============
public void CreatingSketch(UIApplication uiApplication)
{
  Document document = uiApplication.ActiveUIDocument.Document;
  ApplicationServices.Application application = uiApplication.Application;

  // 创建一些几何线，这些线是临时的，所以不需要放在事务中
  XYZ Point1 = XYZ.Zero;
  XYZ Point2 = new XYZ(10, 0, 0);
  XYZ Point3 = new XYZ(10, 10, 0);
  XYZ Point4 = new XYZ(0, 10, 0);

  Line geomLine1 = Line.CreateBound(Point1, Point2);
  Line geomLine2 = Line.CreateBound(Point4, Point3);
  Line geomLine3 = Line.CreateBound(Point1, Point4);

  // 这个几何平面也是临时的，不需要事务
  XYZ origin = XYZ.Zero;
  XYZ normal = new XYZ(0, 0, 1);
  Plane geomPlane = application.Create.NewPlane(normal, origin);

  // 为了创建SketchPlane，我们需要一个事务，因为这个会修改Revit文档模型

  // 任何Transaction要放在 “using”中创建
  // 来保证它被正确的结束，而不会影响到其他地方
  using (Transaction transaction = new Transaction(document))
  {
     if (transaction.Start("Create model curves") == TransactionStatus.Started)
     {
        // 在当前文档中创建一个SketchPlane
        SketchPlane sketch = SketchPlane.Create(document, geomPlane);

        //使用SketchPlane和几何线来创建一个ModelLine
        ModelLine line1 = document.Create.NewModelCurve(geomLine1, sketch) as ModelLine;
        ModelLine line2 = document.Create.NewModelCurve(geomLine2, sketch) as ModelLine;
        ModelLine line3 = document.Create.NewModelCurve(geomLine3, sketch) as ModelLine;

        // 询问用户这个修改是否要提交
        TaskDialog taskDialog = new TaskDialog("Revit");
        taskDialog.MainContent = "Click either [OK] to Commit, or [Cancel] to Roll back the transaction.";
        TaskDialogCommonButtons buttons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
        taskDialog.CommonButtons = buttons;

        if (TaskDialogResult.Ok == taskDialog.Show())
        {
           // 由于种种原因，如果修改或创建的模型不正确，
           // 这个Transaction可能不会被正确提交
           // 如果一个Transaction失败了或被用户取消了，
           // 那么返回的状态就是 RolledBack，而不是Committed。
           if (TransactionStatus.Committed != transaction.Commit())
           {
              TaskDialog.Show("Failure", "Transaction could not be committed");
           }
        }
        else
        {
           transaction.RollBack();
        }
     }
  }
}

//============代码片段2-22：使用Assimilate方法============
public void CompoundOperation(Autodesk.Revit.DB.Document document)
{
  // 所有TransactionGroup要用“using”来创建来保证它的正确结束
  using (TransactionGroup transGroup = new TransactionGroup(document, "Level and Grid"))
  {
     if (transGroup.Start() == TransactionStatus.Started)
     {
        // 我们打算调用两个函数，每个都有一个独立的事务
        // 我们打算这个组合操作要么成功，要么失败
        // 只要其中有一个失败，我们就撤销所有操作

        if (CreateLevel(document, 25.0) && CreateGrid(document, new XYZ(0, 0, 0), new XYZ(10, 0, 0)))
        {
           // Assimilate函数会将这两个事务合并成一个，并只显示TransactionGroup的名字
           // 在Undo菜单里
           transGroup.Assimilate();
        }
        else
        {
           // 如果有一个操作失败了，我们撤销在这个事务组里的所有操作
           transGroup.RollBack();
        }
     }
  }
}

public bool CreateLevel(Autodesk.Revit.DB.Document document, double elevation)
{
  using (Transaction transaction = new Transaction(document, "Creating Level"))
  {
     // 必须启动事务来修改文档
     if (TransactionStatus.Started == transaction.Start())
     {
        if (null != document.Create.NewLevel(elevation))
        {
           return (TransactionStatus.Committed == transaction.Commit());
        }
        // 如果不能创建层，撤销这个事务
        transaction.RollBack();
     }
  }
  return false;
}

public bool CreateGrid(Autodesk.Revit.DB.Document document, XYZ p1, XYZ p2)
{
  using (Transaction transaction = new Transaction(document, "Creating Grid"))
  {
     if (TransactionStatus.Started == transaction.Start())
     {
        Line gridLine = Line.CreateBound(p1, p2);

        if ((null != gridLine) && (null != document.Create.NewGrid(gridLine)))
        {
           if (TransactionStatus.Committed == transaction.Commit())
           {
              return true;
           }
        }
        // 如果不能创建网格，撤销这个事务
        transaction.RollBack();
     }
  }
  return false;
}

//============代码片段2-23：HelloRevit============
using System;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
namespace HelloRevit
{
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   public class Class1 : IExternalCommand
   {
      public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit,
          ref string message, ElementSet elements)
      {
         TaskDialog.Show("Revit", "Hello Revit");
         return Autodesk.Revit.UI.Result.Succeeded;
      }
   }
}

//============代码片段2-24：HelloRevit.addin文件============
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Assembly>[dll文件所在路径]\HelloRevit.dll</Assembly>
    <ClientId>7d4e1893-3a27-4df2-8075-4fa3754537aa</ClientId>
    <FullClassName>HelloRevit.Class1</FullClassName>
    <VendorId>ADSK</VendorId>
  </AddIn>
</RevitAddIns>

//============代码片段2-25：插件抛出异常============
Command: IExternalCommand
{
  public IExternalCommand.Result Execute ()
  {
    //抛出异常…
  }
}

//============代码片段2-26：添加Ribbon面板============
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace HelloRevit
{
   public class CsAddpanel : Autodesk.Revit.UI.IExternalApplication
   {
      public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
      {
         //添加一个新的Ribbon面板
         RibbonPanel ribbonPanel = application.CreateRibbonPanel("NewRibbonPanel");

         //在新的Ribbon面板上添加一个按钮
         //点击这个按钮，前一个例子“HelloRevit”这个插件将被运行。
         PushButton pushButton = ribbonPanel.AddItem(new PushButtonData("HelloRevit",
             "HelloRevit", @"C:\Projects\HelloRevit\HelloRevit.dll", "HelloRevit.Class1")) as PushButton;

         // 给按钮添加一个图片
         Uri uriImage = new Uri(@"C:\Projects\HelloRevit\logo.png");
         BitmapImage largeImage = new BitmapImage(uriImage);
         pushButton.LargeImage = largeImage;

         return Result.Succeeded;
      }

      public Result OnShutdown(UIControlledApplication application)
      {
         return Result.Succeeded;
      }
   }
}

//============代码片段2-27：HelloRevit.addin文件============
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Assembly>C:\Projects\HelloRevit\HelloRevit.dll</Assembly>
    <AddInId>6cdba932-c058-4ec1-b038-33ed590c41d3</AddInId>
    <Name>HelloRevit</Name>
    <FullClassName>HelloRevit.CsAddpanel</FullClassName>
    <VendorId>ADSK</VendorId>
  </AddIn>
</RevitAddIns>


//============代码片段2-28：先选择元素后执行命令============
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
namespace HelloRevit
{
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic)]
   public class Class1 : IExternalCommand
   {
      public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit,
          ref string message, ElementSet elements)
      {

         try
         {
            //在执行该插件之前，先选择一些元素。本例中选择了四面墙，一条模型线，一条网格线，一个房间，一个房间标签。

            //取到当前文档。
            UIDocument uidoc = revit.Application.ActiveUIDocument;
            //取到当前文档的选择集。
            Selection selection = uidoc.Selection;
            ElementSet collection = selection.Elements;

            if (0 == collection.Size)
            {
               // 如果在执行该例子之前没有选择任何元素，则会弹出提示.
               TaskDialog.Show("Revit", "你没有选任何元素.");
            }
            else
            {
               String info = "所选元素类型为: ";
               foreach (Element elem in collection)
               {
                  info += "\n\t" + elem.GetType().ToString();
               }

               TaskDialog.Show("Revit", info);
            }
         }
         catch (Exception e)
         {
            message = e.Message;
            return Autodesk.Revit.UI.Result.Failed;
         }

         return Autodesk.Revit.UI.Result.Succeeded;
      }
   }
}

//============代码片段2-29：在运行外部命令过程中选取元素============
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
namespace HelloRevit
{
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic)]
   public class Class1 : IExternalCommand
   {
      public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit,
          ref string message, ElementSet elements)
      {

         try
         {
            Document document = revit.Application.ActiveUIDocument.Document;
            // 点选指定类型的元素。本例中指定的类型为元素整体。
            Reference pickedElemRef = this.Selection.PickObject(ObjectType.Element);
            // 通过引用取到选中的元素。
            Element elem = this.Document.GetElement(pickedElemRef.ElementId);
            String info = "所选元素类型为: ";
            info += "\n\t" + elem.GetType().ToString();

            TaskDialog.Show("Revit", info);
         }
         catch (Exception e)
         {
            message = e.Message;
            return Autodesk.Revit.UI.Result.Failed;
         }

         return Autodesk.Revit.UI.Result.Succeeded;
      }
   }
}

//============代码片段2-30：通过过滤器取到元素============
using System;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
namespace HelloRevit
{
 [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Automatic)]
   public class Class1 : IExternalCommand
   {
      public Autodesk.Revit.UI.Result Execute(ExternalCommandData revit,
          ref string message, ElementSet elements)
      {
         try
         {
            Document document = revit.Application.ActiveUIDocument.Document;

            // 创建一个类过滤器来过滤出所有的FamilyInstance类的元素。
            ElementClassFilter familyInstanceFilter = new  ElementClassFilter(typeof(FamilyInstance));

            // 创建一个类别过滤器来过滤出所有的内建类型为OST_Doors的元素。
            ElementCategoryFilter doorsCategoryfilter =
                new ElementCategoryFilter(BuiltInCategory.OST_Doors);

            // 创建一个逻辑过滤器来组合前面两个过滤器，实现过滤出所有Door元素。
            LogicalAndFilter doorInstancesFilter =
                new LogicalAndFilter(familyInstanceFilter, doorsCategoryfilter);

            FilteredElementCollector collector = new FilteredElementCollector(document);
            ICollection<ElementId> doors = collector.WherePasses(doorInstancesFilter).ToElementIds();

            String prompt = "The ids of the doors in the current document are:";
            foreach (ElementId id in doors)
            {
               prompt += "\n\t" + id.IntegerValue;
            }

            TaskDialog.Show("Revit", prompt);

         }
         catch (Exception e)
         {
            message = e.Message;
            return Autodesk.Revit.UI.Result.Failed;
         }
         return Autodesk.Revit.UI.Result.Succeeded;
      }
   }
}


//============代码片段3-1通过Id获取元素============
ElementId levelId = new ElementId(30);
Element element = RevitDoc.GetElement(levelId);
Level level = element as Level;
if(level != null)
{
   //使用level
}

//============代码片段3-2 过滤所有外墙============
FilteredElementCollector filteredElements = new FilteredElementCollector(RevitDoc);
ElementClassFilter classFilter = new ElementClassFilter(typeof(Wall));
filteredElements = filteredElements.WherePasses(classFilter);
foreach (Wall wall in filteredElements)
{
   // 获取墙类型“功能”参数，它用来指示墙是否为外墙。
   var functionParameter = wall.WallType.get_Parameter(BuiltInParameter.FUNCTION_PARAM);
   if (functionParameter != null && functionParameter.StorageType == StorageType.Integer)
   {
      if (functionParameter.AsInteger() == (int)WallFunction.Exterior)
      {
         // 使用wall
      }
   }
}

//============代码片段3-3 获取被选元素============
[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
public class MyExternalCommand : Autodesk.Revit.UI.IExternalCommand
{
   public Autodesk.Revit.UI.Result Execute(Autodesk.Revit.UI.ExternalCommandData commandData,
      ref string message, ElementSet elements)
   {
      if (commandData.Application.ActiveUIDocument != null)
      {
         foreach (Element selected in
            commandData.Application.ActiveUIDocument.Selection.Elements)
         {
            Wall wall = selected as Wall;
            if(wall != null)
            {
               //使用wall
            }
         }
      }
      return Autodesk.Revit.UI.Result.Succeeded;
   }
}

//============代码片段3-4 获取“长度”参数============
ParameterSet parameters = element.Parameters;
foreach (Parameter parameter in parameters)
{
   if(parameter.Definition.Name == "长度" && parameter.StorageType == StorageType.Double)
   {
      double length = parameter.AsDouble();
      // 使用length
      break;
   }
}


//============代码片段3-5 使用BuiltInParameter获取长度============
Wall wall = null;
Parameter parameterLength = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
if (parameterLength != null && parameterLength.StorageType == StorageType.Double)
{
   double length = parameterLength.AsDouble();
   // 使用length
}

//============代码片段3-6 修改参数============
Parameter parameterBaseOffset = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
if (parameterBaseOffset != null && parameterBaseOffset.StorageType == StorageType.Double)
{
   if (!parameterBaseOffset.IsReadOnly)
   {
      bool success = parameterBaseOffset.Set(10);
      if (!success)
      {
         // 更新错误报告
      }
   }
   else
   {
      // 参数是只读的
   }
}

//============代码片段3-7 获取共享参数============
// 打开共享参数文件
DefinitionFile definitionFile = RevitApp.OpenSharedParameterFile();
// 获取参数组的集合
DefinitionGroups groups = definitionFile.Groups;

foreach (DefinitionGroup group in groups)
{
   // 获取参数组内的参数定义
   foreach (Definition definition in group.Definitions)
   {
      string name = definition.Name;
      ParameterType type = definition.ParameterType;
      // 对参数定义的其他操作
   }
}

//============代码片段3-8 创建共享参数============
string sharedParametersFilename = @"C:\shared-parameters.txt";
string groupName = "MyGroup";
string definitionName = "MyDefinition";
ParameterType parameterType = ParameterType.Text;
CategorySet categorySet = new CategorySet();
Category wallCategory = RevitDoc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
categorySet.Insert(wallCategory);
bool instanceParameter = true;
BuiltInParameterGroup parameterGroup = BuiltInParameterGroup.PG_DATA;

if (!System.IO.File.Exists(sharedParametersFilename))
{
   try
   {
      System.IO.StreamWriter sw = System.IO.File.CreateText(sharedParametersFilename);
      sw.Close();
   }
   catch (Exception)
   {
      throw new Exception("Can't create shared parameter file: " + sharedParametersFilename);
   }
}
// 设置共享参数文件
RevitApp.SharedParametersFilename = sharedParametersFilename;

// 打开共享参数文件
DefinitionFile definitionFile = RevitApp.OpenSharedParameterFile();
if (definitionFile == null)
{
   throw new Exception("Can not open shared parameter file!");
}

// 获取参数组的集合
DefinitionGroups groups = definitionFile.Groups;

// 获取参数组
DefinitionGroup group = groups.get_Item(groupName);
if (null == group)
{
   // 如果参数组不存在，则创建一个
   group = groups.Create(groupName);
}
if (null == group)
   throw new Exception("Failed to get or create group: " + groupName);

// 获取参数定义
Definition definition = group.Definitions.get_Item(definitionName);
if (definition == null)
{
   // 如果参数定义不存在，则创建一个
   definition = group.Definitions.Create(definitionName, parameterType);
}

// 调用不同的函数创建类型参数或者实例参数
ElementBinding binding = null;
if (instanceParameter)
{
   binding = RevitApp.Create.NewInstanceBinding(categorySet);
}
else
{
   binding = RevitApp.Create.NewTypeBinding(categorySet);
}

// 把参数定义和类别绑定起来（下面的小节会提到“绑定”），元素的新的参数就创建成功了。
bool insertSuccess = RevitDoc.ParameterBindings.Insert(definition, binding, parameterGroup);

if (!insertSuccess)
{
   throw new Exception("Failed to bind definition to category");
}

//============代码片段3-9 获取类别和参数的绑定============
BindingMap map = RevitDoc.ParameterBindings;
DefinitionBindingMapIterator dep = map.ForwardIterator();
while (dep.MoveNext())
{
   Definition definition = dep.Key;
   // 获取参数定义的基本信息
   string definitionName = definition.Name;
   ParameterType parameterType = definition.ParameterType;
   // 几乎都可以转型为InstanceBinding，笔者没有碰到过其他情况，如有例外，请联系我们。
   InstanceBinding instanceBinding = dep.Current as InstanceBinding;
   if (instanceBinding != null)
   {
      // 获取绑定的类别列表
      CategorySet categorySet = instanceBinding.Categories;
   }
}

//============代码片段3-10 判断共享参数和项目参数============
Parameter parameter;
InternalDefinition definition = parameter.Definition as InternalDefinition;

bool isSharedParameter = parameter.IsShared; //共享参数

bool isProjectParameter = definition.BuiltInParameter == BuiltInParameter.INVALID && !parameter.IsShared; //项目参数

//============代码片段3-11 获取分析模型的几何信息============
Element element = RevitDoc.GetElement(new ElementId(183554));
if (element == null) return;
AnalyticalModel analyticalModel = element.GetAnalyticalModel();
if(analyticalModel.IsSingleCurve())
{
   Curve curve = analyticalModel.GetCurve();
   // work with curve
}
else if(analyticalModel.IsSinglePoint())
{
   XYZ p = analyticalModel.GetPoint();
   // work with point
}
else
{
   IList<Curve> curves = analyticalModel.GetCurves(AnalyticalCurveType.ActiveCurves);
   // work with curves
}

//============代码片段3-12 放置类型为"0762 x 2032 mm"的门============
string doorTypeName = "0762 x 2032 mm";
FamilySymbol doorType = null;

// 在文档中找到名字为"0762 x 2032 mm"的门类型
ElementFilter doorCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
ElementFilter familySymbolFilter = new ElementClassFilter(typeof(FamilySymbol));
LogicalAndFilter andFilter = new LogicalAndFilter(doorCategoryFilter, familySymbolFilter);
FilteredElementCollector doorSymbols = new FilteredElementCollector(RevitDoc);
doorSymbols = doorSymbols.WherePasses(andFilter);
bool symbolFound = false;
foreach (FamilySymbol element in doorSymbols)
{
   if (element.Name == doorTypeName)
   {
      symbolFound = true;
      doorType = element;
      break;
   }
}

// 如果没有找到，就加载一个族文件
if (!symbolFound)
{
   string file = @"C:\ProgramData\Autodesk\RVT 2014\Libraries\Chinese_INTL\门\M_单-嵌板 4.rfa";
   Family family;
   bool loadSuccess = RevitDoc.LoadFamily(file, out family);
   if (loadSuccess)
   {
      foreach (ElementId doorTypeId in family.GetValidTypes())
      {
         doorType = RevitDoc.GetElement(doorTypeId) as FamilySymbol;
         if (doorType != null)
         {
            if (doorType.Name == doorTypeName)
            {
               break;
            }
         }
      }
   }
   else
   {
      Autodesk.Revit.UI.TaskDialog.Show("Load family failed", "Could not load family file '" + file + "'");
   }
}

// 使用族类型创建门
if (doorType != null)
{
   // 首先找到线形的墙
   ElementFilter wallFilter = new ElementClassFilter(typeof(Wall));
   FilteredElementCollector filteredElements = new FilteredElementCollector(RevitDoc);
   filteredElements = filteredElements.WherePasses(wallFilter);
   Wall wall = null;
   Line line = null;
   foreach (Wall element in filteredElements)
   {
      LocationCurve locationCurve = element.Location as LocationCurve;
      if (locationCurve != null)
      {
         line = locationCurve.Curve as Line;
         if (line != null)
         {
            wall = element;
            break;
         }
      }
   }

   // 在墙的中心位置创建一个门
   if (wall != null)
   {
      XYZ midPoint = (line.get_EndPoint(0) + line.get_EndPoint(1)) / 2;
      Level wallLevel = RevitDoc.GetElement(wall.LevelId) as Level;
      //创建门：传入标高参数，作为门的默认标高
      FamilyInstance door = RevitDoc.Create.NewFamilyInstance(midPoint, doorType, wall, wallLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
      Autodesk.Revit.UI.TaskDialog.Show("Succeed", door.Id.ToString());
      Trace.WriteLine("Door created: " + door.Id.ToString());
   }
   else
   {
      Autodesk.Revit.UI.TaskDialog.Show("元素不存在", "没有找到符合条件的墙");
   }
}
else
{
   Autodesk.Revit.UI.TaskDialog.Show("族类型不存在", "没有找到族类型'" + doorTypeName + "'");
}

//============代码片段3-13 创建拉伸实体族============
//创建族文档
Document familyDoc = RevitApp.NewFamilyDocument(@"C:\ProgramData\Autodesk\RVT 2014\Family Templates\Chinese\公制常规模型.rft");
using (Transaction transaction = new Transaction(familyDoc))
{
   transaction.Start("Create family");
   CurveArray curveArray = new CurveArray();
   curveArray.Append(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(5, 0, 0)));
   curveArray.Append(Line.CreateBound(new XYZ(5, 0, 0), new XYZ(5, 5, 0)));
   curveArray.Append(Line.CreateBound(new XYZ(5, 5, 0), new XYZ(0, 5, 0)));
   curveArray.Append(Line.CreateBound(new XYZ(0, 5, 0), new XYZ(0, 0, 0)));
   CurveArrArray curveArrArray = new CurveArrArray();
   curveArrArray.Append(curveArray);
   //创建一个拉伸实体
   familyDoc.FamilyCreate.NewExtrusion(true, curveArrArray, SketchPlane.Create(familyDoc, RevitApp.Create.NewPlane(new XYZ(0, 0, 1), XYZ.Zero)), 10);
   //创建一个族类型
   familyDoc.FamilyManager.NewType("MyNewType");
   transaction.Commit();
   familyDoc.SaveAs("MyNewFamily.rfa");
   familyDoc.Close();
}

//============代码片段3-14 复制墙类型============
Wall wall = RevitDoc.GetElement(new ElementId(185521)) as Wall;
WallType wallType = wall.WallType;
ElementType duplicatedWallType = wallType.Duplicate(wallType.Name + " (duplicated)");

//============代码片段3-15：元素编辑============
Document projectDoc = ActiveUIDocument.Document;
           
using(Transaction moveColumnTran = new Transaction(projectDoc, "Move a new column to the new place"))
{
 moveColumnTran.Start();
               
 // 获取Revit文档的创建句柄
 Autodesk.Revit.Creation.Document creater = projectDoc.Create;
 // 创建一根柱子：使用给定的位置（坐标原点），柱子类型和标高（高度为0）
 XYZ origin = new XYZ(0, 0, 0);
 Level level = GetALevel(projectDoc);
 FamilySymbol columnType = GetAColumnType(projectDoc);
 FamilyInstance column = creater.NewFamilyInstance(origin, columnType, level, Autodesk.Revit.DB.Structure.StructuralType.Column);
 // 把柱子移动到新的位置
 XYZ newPlace = new XYZ(10, 20, 30);
 ElementTransformUtils.MoveElement(projectDoc, column.Id, newPlace);
           
 moveColumnTran.Commit();       
}

//============代码片段3-16：元素编辑============
Wall wall = element as Wall;
if (null != wall)
{
    LocationCurve wallLine = wall.Location as LocationCurve;
    XYZ newPlace = new XYZ(10, 20, 0);
    wallLine.Move(newPlace);
}

//============代码片段3-17：元素编辑============
using(Transaction tran = new Transaction(projectDoc, "Change the wall's curve with a new location line."))
{
 tran.Start();
               
 LocationCurve wallLine = wall.Location as LocationCurve;
 XYZ p1 = XYZ.Zero;
 XYZ p2 = new XYZ(10, 20, 0);
 Line newWallLine = Line.CreateBound(p1, p2);
 // 把墙的位置线换成新的线
 wallLine.Curve = newWallLine;
            
 tran.Commit();
}

//============代码片段3-18：元素编辑============
FamilyInstance column = element as FamilyInstance;
if (null != column)
{
    LocationPoint columnPoint = column.Location as LocationPoint;
    XYZ newLocation = new XYZ(10, 20, 0);
    // 移动柱子到新的位置（10,20,0）
    columnPoint.Point = newLocation;
}

//============代码片段3-19：元素编辑============
using(Transaction tran = new Transaction(projectDoc, "Rotate the wall."))
{
  tran.Start();
  LocationCurve wallLine = wall.Location as LocationCurve;
  XYZ p1 = wallLine.Curve.GetEndPoint(0);
  XYZ p2 = new XYZ(p1.X, p1.Y, 30);
  Line axis = Line.CreateBound(p1, p2);
  ElementTransformUtils.RotateElement(projectDoc, wall.Id, axis, Math.PI / 3.0);
  tran.Commit();
}

//============代码片段3-20：元素编辑============
Document projectDoc = ActiveUIDocument.Document;
            
using(Transaction tran = new Transaction(projectDoc, "Rotate the wall and the column."))
{
   tran.Start();
                
   Wall wall = projectDoc.GetElement(new ElementId(184163)) as Wall;
                
   XYZ aa = XYZ.Zero;
   XYZ cc = XYZ.Zero;
   // 通过元素的位置线来旋转元素
   LocationCurve curve = wall.Location as LocationCurve;
   if (null != curve)
   {
       Curve line = curve.Curve;
       aa = line.GetEndPoint(0);
       cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
       Line axis = Line.CreateBound(aa, cc);
       curve.Rotate(axis, Math.PI / 2.0);
   }
                
   FamilyInstance column = projectDoc.GetElement(new ElementId(184150)) as FamilyInstance;
                
   // 通过元素的位置点来旋转元素
   LocationPoint point = column.Location as LocationPoint;
   if (null != point)
   {
        aa = point.Point;
        cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
        Line axis = Line.CreateBound(aa, cc);
        point.Rotate(axis, Math.PI / 3.0);
   }
   tran.Commit();
}

//============代码片段3-21：元素编辑============
using(Transaction tran = new Transaction(projectDoc, "Mirror the column."))
{
  tran.Start();
                
  FamilyInstance column = projectDoc.GetElement(new ElementId(184150)) as FamilyInstance;
  if (null != column)
  {
     Plane plane = new Plane(XYZ.BasisX, XYZ.Zero);
     if(ElementTransformUtils.CanMirrorElement(projectDoc, column.Id))
     {
        ElementTransformUtils.MirrorElement(projectDoc, column.Id, plane);
     }
  }
  tran.Commit();
}

//============代码片段3-22：元素编辑============
Document projectDoc = ActiveUIDocument.Document;
Wall wall = projectDoc.GetElement(new ElementId(184163)) as Wall;
using(Transaction tran = new Transaction(projectDoc, "Delete the wall."))
{
    tran.Start();
    // 删除选择的元素：墙
    ICollection<ElementId> deletedElements = projectDoc.Delete(wall.Id);
    tran.Commit();
}

//============代码片段3-23：元素编辑============
Document projectDoc = ActiveUIDocument.Document;

List<ElementId> elementsToDelete = new List<ElementId>();            
 using(Transaction tran = new Transaction(projectDoc, "Delete the selected elements."))
 {
     tran.Start();
     foreach (Element elem in ActiveUIDocument.Selection.Elements)
     {
         elementsToDelete.Add(elem.Id);
     }

      ICollection<ElementId> deletedElements = projectDoc.Delete(elementsToDelete);
      tran.Commit();
 }

//============代码片段3-24：元素编辑============
Document projectDoc = ActiveUIDocument.Document;
List<ElementId> elementsToGroup = new List<ElementId>();
using(Transaction tran = new Transaction(projectDoc, "Group the selected elements."))
{
tran.Start();
foreach (Element elem in ActiveUIDocument.Selection.Elements)
{
   elementsToGroup.Add(elem.Id);
}

Group group = projectDoc.Create.NewGroup(elementsToGroup);
tran.Commit();
}

//============代码片段3-25：元素编辑============
         // 把默认的组合名字改成新的名字：“MyGroup”
  group.GroupType.Name = "MyGroup";

//============代码片段3-26：元素编辑============
 public void ArraryElements()
 {
    Document projectDoc = ActiveUIDocument.Document;
    Wall wall = projectDoc.GetElement(new ElementId(2307)) as Wall;
    using(Transaction tran = new Transaction(projectDoc, "LinearArray the wall."))
    {
       tran.Start();
       XYZ translation = new XYZ(0,10,0);
       LinearArray.Create(projectDoc, projectDoc.ActiveView, wall.Id, 3, translation, ArrayAnchorMember.Second);
       tran.Commit();
     }
 }

//============代码片段3-27：元素编辑============
class projectFamLoadOption : IFamilyLoadOptions
{
    bool IFamilyLoadOptions.OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
    {
        overwriteParameterValues = true;
        return true;
    }
    bool IFamilyLoadOptions.OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
    {
        source = FamilySource.Project;
        overwriteParameterValues = true;
        return true;
    }
};

//============代码片段3-28：元素编辑============
       Document projectDoc = ActiveUIDocument.Document;
// 这里是自定义族实例，比如门，窗，桌子…
         FamilyInstance famInst = elem as FamilyInstance;
// 编辑族，拿到族文档
Document familyDoc = projectDoc.EditFamily(famInst.Symbol.Family);
// 在族文档中添加一个新的参数
using(Transaction tran = new Transaction(projectDoc, "Edit family Document."))
{
  tran.Start();
  string paramName = "MyParam ";
  familyDoc.FamilyManager.AddParameter(paramName, BuiltInParameterGroup.PG_TEXT, ParameterType.Text, false);
  tran.Commit();
}
// 将这些修改重新载入到工程文档中
Family loadedFamily = familyDoc.LoadFamily(RevitDoc, new projectFamLoadOption());

//============代码片段3-29：元素编辑============
public void CreatReferencePlane()
{
   Document doc = this.ActiveUIDocument.Document;    
   if(!doc.IsFamilyDocument)
      return;
            
   using(Transaction transaction = new Transaction(doc, "Editing Family"))
   {
      transaction.Start();
      XYZ bubbleEnd = new XYZ(0,5,5);
      XYZ freeEnd = new XYZ(5, 5, 5);
      XYZ cutVector = XYZ.BasisY;
      View view = doc.ActiveView;
      ReferencePlane referencePlane = doc.FamilyCreate.NewReferencePlane(bubbleEnd, freeEnd, cutVector, view);
      referencePlane.Name = "MyReferencePlane";
      transaction.Commit();                
   }
}

//============代码片段3-30：元素编辑============
public void ChangeModelCurveToReferenceLine()
{            
    Document doc = this.ActiveUIDocument.Document;    
    ModelCurve modelCurve = doc.GetElement(new ElementId(2910)) as ModelCurve;
    using(Transaction transaction = new Transaction(doc, "Change model curve to reference line."))
    {
        transaction.Start();
        modelCurve.ChangeToReferenceLine();
        transaction.Commit();                
    }            
}

//============代码片段3-31：元素编辑============
public void CreateModelCurve()
{
   Document doc = this.ActiveUIDocument.Document;    
   // 在族文档中找到名字为"Ref. Level"的标高
   FilteredElementCollector collector = new FilteredElementCollector(doc);
   collector = collector.OfCategory(BuiltInCategory.OST_Levels);
   var levelElements = from element in collector where element.Name == "Ref. Level" select element;  
   List<Autodesk.Revit.DB.Element> levels = levelElements.ToList<Autodesk.Revit.DB.Element>();        
   if(levels.Count <= 0)
      return;            
   Level refLevel = levels[0] as Level;
            
   // 创建一条几何直线，一个基于标高的草图平面，然后在这个草图平面上创建一条模型线.
   using(Transaction trans = new Transaction(doc, "Create model line."))
   {
      trans.Start();    
      Line line = Line.CreateBound(XYZ.Zero, new XYZ(10,10,0));
      SketchPlane sketchPlane = SketchPlane.Create(doc, refLevel.Id);        
      ModelCurve modelLine = doc.FamilyCreate.NewModelCurve(line, sketchPlane);
     trans.Commit();
   }
}

//============代码片段3-32：元素编辑============
public void CreateSketchPlaneByPlane()
{
   Document doc = this.ActiveUIDocument.Document;    
   using(Transaction trans = new Transaction(doc, "Create model arc."))
   {
     trans.Start();    
     Plane plane = this.Application.Create.NewPlane(XYZ.BasisZ, XYZ.Zero);
     SketchPlane sketchPlane = SketchPlane.Create(doc, plane);
            
     Arc arc = Arc.Create(plane, 5, 0, Math.PI * 2);
     ModelCurve modelCircle = doc.FamilyCreate.NewModelCurve(arc, sketchPlane);
     trans.Commit();
   }
}

//============代码片段3-33：元素编辑============
public void GetSketchFromExtrusion()
{
    Document doc = this.ActiveUIDocument.Document;
    Extrusion extrusion = doc.GetElement(new ElementId(3388)) as Extrusion;
    SketchPlane sketchPlane = extrusion.Sketch.SketchPlane;
    CurveArrArray sketchProfile = extrusion.Sketch.Profile;
}

//============代码片段3-34：元素过滤器============
FilteredElementCollector collection = new FilteredElementCollector(RevitDoc);
ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StackedWalls);
collection.OfClass(typeof(Wall)).WherePasses(filter);
ICollection<ElementId> foundIds = collection.ToElementIds();

//============代码片段3-35：元素过滤器============
FilteredElementCollector collector = new FilteredElementCollector(m_doc);
 // 查询并遍历文档中所有的Level
collector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Levels)).WhereElementIsNotElementType();
foreach(Level level in collector)
{
    TaskDialog.Show("Level Name", level.Name);
}

//============代码片段3-36：元素过滤器============
FilteredElementCollector collector = new FilteredElementCollector(m_doc);
 
// 首先使用一个内建的过滤器来减少后面使用LINQ查询的元素数量
collector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Levels));

// LINQ查询：找到名字为"Level 1"的标高
var levelElements = from element in collector
                    where element.Name == "Level 1"
                    select element;  
List<Autodesk.Revit.DB.Element> levels = levelElements.ToList<Autodesk.Revit.DB.Element>();
 
ElementId level1Id = levels[0].Id;

//============代码片段3-37：元素过滤器============
/// <summary>
/// 使用ElementCategoryFilter过滤元素
/// </summary>
void TestElementCategoryFilter(Document doc)
{
   // 找到所有属于墙类别的元素：墙实例和墙类型都将会被过滤出来
   FilteredElementCollector collector = new FilteredElementCollector(doc);
   ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
   ICollection<Element> founds = collector.WherePasses(filter).ToElements();
   foreach (Element elem in founds)
   {
     Trace.WriteLine(String.Format("  Element id: {0}, type: {1}", elem.Id.IntegerValue, elem.GetType().Name));
   }
}

//============代码片段3-38：元素过滤器============
/// <summary>
/// 使用ElementClassFilter过滤元素
/// </summary>
void TestElementClassFilter(Document doc)
{
  // 找到所有属于FamilySymbol的元素：元素的子类也将被过滤出来
  FilteredElementCollector collector = new FilteredElementCollector(doc);
  ElementClassFilter filter = new ElementClassFilter(typeof(FamilySymbol));
  ICollection<ElementId> founds = collector.WherePasses(filter).ToElementIds();
  Trace.WriteLine(String.Format("  Found {0} FamilySymbols.", founds.Count));
}

//============代码片段3-39：元素过滤器============
/// <summary>
/// 使用ElementIsElementTypeFilter过滤元素
/// </summary>
void TestElementIsElementTypeFilter(Document doc)
{
  // 找到所有属于ElementType的元素
  FilteredElementCollector collector = new FilteredElementCollector(doc);
  ElementIsElementTypeFilter filter = new ElementIsElementTypeFilter();
  ICollection<ElementId> founds = collector.WherePasses(filter).ToElementIds();
    Trace.WriteLine(String.Format("  Found {0} ElementTypes.", founds.Count));
}

//============代码片段3-40：元素过滤器============
/// <summary>
/// 使用FamilySymbolFilter过滤元素
/// </summary>
void TestFamilySymbolFilter(Document doc)
{
  // 找到当前文档中族实例所对应的族类型
  FilteredElementCollector collector = new FilteredElementCollector(doc);
  ICollection<ElementId> famIds = collector.OfClass(typeof(Family)).ToElementIds();
  foreach (ElementId famId in famIds)
  {
    collector = new FilteredElementCollector(doc);
    FamilySymbolFilter filter = new FamilySymbolFilter(famId);
    int count = collector.WherePasses(filter).ToElementIds().Count;
    Trace.WriteLine(String.Format("  {0} FamilySybmols belong to Family {1}.", count, famId.IntegerValue));
  }
}

//============代码片段3-41：元素过滤器============
/// <summary>
/// 使用ExclusionFilter过滤元素
/// </summary>
void TestExclusionFilter(Document doc)
{
  // 找到所有除族类型FamilySymbol外的元素类型ElementType
  FilteredElementCollector collector = new FilteredElementCollector(doc);
  ICollection<ElementId> excludes = collector.OfClass(typeof(FamilySymbol)).ToElementIds();
    
  // 创建一个排除族类型FamilySymbol的过滤器
  ExclusionFilter filter = new ExclusionFilter(excludes);
  ICollection<ElementId> founds = collector.WhereElementIsElementType().WherePasses(filter).ToElementIds();
  Trace.WriteLine(String.Format("  Found {0} ElementTypes which are not FamilySybmols", founds.Count));
}

//============代码片段3-42：元素过滤器============
/// <summary>
/// 使用ElementLevelFilter过滤元素
/// </summary>
void TestElementLevelFilter(Document doc)
{
  // 找到当前所有标高对应的所有元素
  FilteredElementCollector collector = new FilteredElementCollector(doc);
  ICollection<ElementId> levelIds = collector.OfClass(typeof(Level)).ToElementIds();
  foreach (ElementId levelId in levelIds)
  {
    collector = new FilteredElementCollector(doc);
    ElementLevelFilter filter = new ElementLevelFilter(levelId);
    ICollection<ElementId> founds = collector.WherePasses(filter).ToElementIds();
    Trace.WriteLine(String.Format("  {0} Elements are associated to Level {1}.", founds.Count, levelId.IntegerValue));
  }
}

//============代码片段3-43：元素过滤器============
/// <summary>
/// 使用ElementParameterFilter过滤元素
/// </summary>
void TestElementParameterFilter(Document doc)
{
  // 找到所有id大于99的元素
  BuiltInParameter testParam = BuiltInParameter.ID_PARAM;
  // 提供者
  ParameterValueProvider pvp = new ParameterValueProvider(new ElementId((int)testParam));
  // 评估者
  FilterNumericRuleEvaluator fnrv = new FilterNumericGreater();
  // 规则值   
  ElementId ruleValId = new ElementId(99); // Id 大于 99
  // 创建规则过滤器及对应的元素参数过滤器
  FilterRule fRule = new FilterElementIdRule(pvp, fnrv, ruleValId);
  ElementParameterFilter filter = new ElementParameterFilter(fRule);
  FilteredElementCollector collector = new FilteredElementCollector(doc);
  ICollection<Element> founds = collector.WherePasses(filter).ToElements();
  foreach (Element elem in founds)
  {
    Trace.WriteLine(String.Format("  Element id: {0}", elem.Id.IntegerValue));
  }
}

//============代码片段3-44：元素过滤器============
/// <summary>
/// 使用FamilyInstanceFilter过滤元素
/// </summary>
void TestFamilyInstanceFilter(Document doc)
{
  // 找到名字"W10X49"的族类型
  FilteredElementCollector collector = new FilteredElementCollector(Document);
  collector = collector.OfClass(typeof(FamilySymbol));
  var query = from element in collector
      where element.Name == "W10X49"
      select element; // Linq 查询
  List<Autodesk.Revit.DB.Element> famSyms = query.ToList<Autodesk.Revit.DB.Element>();
  ElementId symbolId = famSyms[0].Id;
    
  // 创建过滤器并找到该族类型对应的所有族实例
  collector = new FilteredElementCollector(doc);
  FamilyInstanceFilter filter = new FamilyInstanceFilter(doc, symbolId);
  ICollection<Element> founds = collector.WherePasses(filter).ToElements();
  foreach (FamilyInstance inst in founds)
  {
    Trace.WriteLine(String.Format("  FamilyInstance {0}, FamilySybmol Id {1}, Name: {2}",inst.Id.IntegerValue, inst.Symbol.Id.IntegerValue, inst.Symbol.Name));
  }
}

//============代码片段3-45：元素过滤器============
      ///  /// <summary>
/// 使用CurveElementFilter 过滤元素
/// </summary>
void TestCurveElementFilter(Document doc)
{
    // 找到所有线元素类型对应的线型元素
    Array stTypes = Enum.GetValues(typeof(CurveElementType));
    foreach (CurveElementType tstType in stTypes)
    {
        if (tstType == CurveElementType.Invalid) continue;
        FilteredElementCollector collector = new FilteredElementCollector(Document);
        CurveElementFilter filter = new CurveElementFilter(tstType);
        int foundNum = collector.WherePasses(filter).ToElementIds().Count;
        Trace.WriteLine(String.Format(" {0}: elements amount {1}", tstType.GetType().Name, foundNum));
    }
}

//============代码片段3-46：元素过滤器============
/// <summary>
/// 使用LogicalOrFilter过滤元素
/// </summary>
void TestLogicalOrFilter(Document doc)
{
  // 情形 1: 合并两个过滤器 ->
    // 找到所有属于墙类别或者属于标高类别的元素
  ElementCategoryFilter filterWall = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
  ElementCategoryFilter filterLevel = new ElementCategoryFilter(BuiltInCategory.OST_Levels);
  LogicalOrFilter orFilter = new LogicalOrFilter(filterWall, filterLevel);
  FilteredElementCollector collector = new FilteredElementCollector(doc);
  ICollection<Element> founds = collector.WherePasses(orFilter).ToElements();
  foreach(Element elem in founds)
  {
    Trace.WriteLine(String.Format("  Element Id {0}, type {1}", elem.Id.IntegerValue, elem.GetType()));
  }
    
  // 情形 2: 合并两个过滤器集合 -> 找到所有属于传入类型的元素
  Type[] elemTypes = { typeof(Wall), typeof(Level), typeof(Floor), typeof(Rebar), typeof(MEPSystem)};
  List<ElementFilter> filterSet = new List<ElementFilter>();
  foreach (Type elemType in elemTypes)
  {
    ElementClassFilter filter = new ElementClassFilter(elemType);
    filterSet.Add(filter);
  }
  orFilter = new LogicalOrFilter(filterSet);
  collector = new FilteredElementCollector(doc);
  founds = collector.WherePasses(orFilter).ToElements();
  foreach (Element elem in founds)
  {
    Trace.WriteLine(String.Format("  Element Id {0}, type {1}", elem.Id.IntegerValue, elem.GetType().Name));
  }
}

//============代码片段3-47：元素过滤器============
/// <summary>
/// 使用LogicalAndFilter过滤器
/// </summary>
void TestLogicalAndFilter(Document doc)
{
  // 情形 1: 合并两个过滤器 -> 找到所有符合特定设计选项的墙
  ElementClassFilter wallFilter = new ElementClassFilter(typeof(Wall));
  FilteredElementCollector collector = new FilteredElementCollector(doc);
  ICollection<ElementId> designOptIds = collector.OfClass(typeof(DesignOption)).ToElementIds();
  foreach(ElementId curId in designOptIds)
  {
    ElementDesignOptionFilter designFilter = new ElementDesignOptionFilter(curId);
    LogicalAndFilter andFilter = new LogicalAndFilter(wallFilter, designFilter);
    collector = new FilteredElementCollector(doc);
    int wallCount = collector.WherePasses(andFilter).ToElementIds().Count;
    Trace.WriteLine(String.Format("  {0} Walls belong to DesignOption {1}.", wallCount, curId.IntegerValue));
  }
    
  // 情形 2: 找到所有符合特定设计选项并且其StructuralWallUsage 属于承重的墙
  foreach (ElementId curId in designOptIds)
  {
    // 构造逻辑与过滤器
    List<ElementFilter> filters = new List<ElementFilter>();
    filters.Add(wallFilter);
    filters.Add(new ElementDesignOptionFilter(curId));
    filters.Add(new StructuralWallUsageFilter(StructuralWallUsage.Bearing));
    LogicalAndFilter andFilter = new LogicalAndFilter(filters);
        
    // 应用该过滤器并遍历获取到的元素
    collector = new FilteredElementCollector(doc);
    int wallCount = collector.WherePasses(andFilter).ToElementIds().Count;
    Trace.WriteLine(String.Format("  {0} Bearing Walls belong to DesignOption {1}.", wallCount, curId.IntegerValue));
  }
}

//============代码片段3-48：元素过滤器============
  FilteredElementCollector collector = new FilteredElementCollector(document);
  // 找到所有符合某种特定设计选项的墙
  optionICollection<ElementId> walls =    collector.OfClass(typeof(Wall)).ContainedInDesignOption(myDesignOptionId).ToElementIds();

//============代码片段4-1 修改标高的基面============
LevelType levelType = RevitDoc.GetElement(level.GetTypeId()) as LevelType;
Parameter relativeBaseType = levelType.get_Parameter(BuiltInParameter.LEVEL_RELATIVE_BASE_TYPE);
relativeBaseType.Set(1); //项目基点 = 0, 测量点 = 1

//============代码片段4-2 创建标高============
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create Level");
   Level level = RevitDoc.Create.NewLevel(10.0);
   transaction.Commit();
}

//============代码片段4-3 创建标高对应的视图============
Level level; //已知的标高
//过滤出所有的ViewFamilyType
var classFilter = new ElementClassFilter(typeof(ViewFamilyType));
FilteredElementCollector filteredElements = new FilteredElementCollector(RevitDoc);
filteredElements = filteredElements.WherePasses(classFilter);
foreach (ViewFamilyType viewFamilyType in filteredElements)
{
   //找到ViewFamily类型是FloorPlan或者CeilingPlan的ViewFamilyType
   if (viewFamilyType.ViewFamily == ViewFamily.FloorPlan ||
      viewFamilyType.ViewFamily == ViewFamily.CeilingPlan)
   {
      transaction.Start("Create view of type " + viewFamilyType.ViewFamily);
      //创建视图
      ViewPlan view = ViewPlan.Create(RevitDoc, viewFamilyType.Id, level.Id);
      transaction.Commit();
   }
}

//============代码片段4-4 创建轴网============
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create Grid");
   Grid grid = RevitDoc.Create.NewGrid(
      Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 10, 0)));
   grid.Name = "BB";
   transaction.Commit();
}

//============代码片段4-5 获取墙每层厚度和材质============
Wall wall = GetElement(RevitDoc, new ElementId(732980)) as Wall;
CompoundStructure compoundStructure = wall.WallType.GetCompoundStructure();
if (compoundStructure == null)
   return;
if (compoundStructure.LayerCount > 0)
{
   foreach (CompoundStructureLayer compoundStructureLayer in compoundStructure.GetLayers())
   {
      //获取材质和厚度
      ElementId materialId = compoundStructureLayer.MaterialId;
      double layerWidth = compoundStructureLayer.Width;
   }
}

//============代码片段4-6 获取楼板的上表面============
Floor floor = GetElement<Floor>(185601);

// 获取一个楼板面的引用
IList<Reference> references = HostObjectUtils.GetTopFaces(floor);
if (references.Count == 1)
{
   var reference = references[0];

   // 从引用获取面的几何对象，这里是一个PlanarFace
   GeometryObject topFaceGeo = floor.GetGeometryObjectFromReference(reference);
   // 转型成我们想要的对象
   PlanarFace topFace = topFaceGeo as PlanarFace;
}

//============代码片段4-7 创建默认墙============
ElementId levelId = new ElementId(311);
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create wall");
   Wall wall = Wall.Create(RevitDoc, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(100, 0, 0)), levelId, false);
   transaction.Commit();
}

//============代码片段4-8 创建梯形墙============
IList<Curve> curves = new List<Curve>();
curves.Add(Line.CreateBound(new XYZ(100, 20, 0), new XYZ(100, -20, 0)));
curves.Add(Line.CreateBound(new XYZ(100, -20, 0), new XYZ(100, -10, 10)));
curves.Add(Line.CreateBound(new XYZ(100, -10, 10), new XYZ(100, 10, 10)));
curves.Add(Line.CreateBound(new XYZ(100, 10, 5), new XYZ(100, 20, 0)));
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create wall");
   Wall wall = Wall.Create(RevitDoc, curves, false);
   transaction.Commit();
}

//============代码片段4-9 创建正反两面墙============
ElementId levelId = new ElementId(311);
ElementId wallTypeId = new ElementId(397);
IList<Curve> curves = new List<Curve>();

// 创建第一面墙
XYZ[] vertexes = new XYZ[] { new XYZ(0, 0, 0), new XYZ(0, 100, 0), new XYZ(0, 0, 100) };
for (int ii = 0; ii < vertexes.Length; ii++)
{
   if (ii != vertexes.Length - 1)
      curves.Add(Line.CreateBound(vertexes[ii], vertexes[ii + 1]));
   else
      curves.Add(Line.CreateBound(vertexes[ii], vertexes[0]));
}
Wall wall = null;
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create wall 1");
   wall = Wall.Create(RevitDoc, curves, wallTypeId, levelId, false, new XYZ(-1, 0, 0));
   transaction.Commit();
}

// 创建第二面墙，面朝向相反
curves.Clear();
vertexes = new XYZ[] { new XYZ(0, 0, 100), new XYZ(0, 100, 100), new XYZ(0, 100, 0) };
for (int ii = 0; ii < vertexes.Length; ii++)
{
   if (ii != vertexes.Length - 1)
      curves.Add(Line.CreateBound(vertexes[ii], vertexes[ii + 1]));
   else
      curves.Add(Line.CreateBound(vertexes[ii], vertexes[0]));
}
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create wall 2");
   wall = Wall.Create(RevitDoc, curves, wallTypeId, levelId, false, new XYZ(1, 0, 0));
   transaction.Commit();
}

//============代码片段4-10 创建墙，并设置高度，偏移和是否翻转============
ElementId levelId = new ElementId(311);
ElementId wallTypeId = new ElementId(397);

using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create wall");
   Wall wall = Wall.Create(RevitDoc, Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 100, 0)), wallTypeId, levelId, 200, 300, true, false);
   transaction.Commit();
}

//============代码片段4-11 创建三角形墙============
CurveArray curveArray = new CurveArray();
curveArray.Append(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(100, 0, 0)));
curveArray.Append(Line.CreateBound(new XYZ(100, 0, 0), new XYZ(0, 100, 0)));
curveArray.Append(Line.CreateBound(new XYZ(0, 100, 0), new XYZ(0, 0, 0)));
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create floor");
   Floor floor = RevitDoc.Create.NewFloor(curveArray, false);
   transaction.Commit();
}

//============代码片段4-12 创建屋顶============
using (Transaction transaction = new Transaction(RevitDoc))
{
   View view = RevitDoc.ActiveView;
   //先创建一个参照平面
   XYZ bubbleEnd = new XYZ(0, 0, 0);
   XYZ freeEnd = new XYZ(0, 100, 0);
   XYZ thirdPnt = new XYZ(0, 0, 100);
   transaction.Start("Create reference plane");
   ReferencePlane plane =
      RevitDoc.Create.NewReferencePlane2(bubbleEnd, freeEnd, thirdPnt, view);
   transaction.Commit();
   //创建屋顶前准备参数
   Level level = RevitDoc.GetElement(new ElementId(311)) as Level;
   RoofType roofType = RevitDoc.GetElement(new ElementId(335)) as RoofType;
   CurveArray curveArray = new CurveArray();
   curveArray.Append(Line.CreateBound(new XYZ(0, 0, 50), new XYZ(0, 50, 100)));
   curveArray.Append(Line.CreateBound(new XYZ(0, 50, 100), new XYZ(0, 100, 50)));
   //创建屋顶
   transaction.Start("Create roof");
   RevitDoc.Create.NewExtrusionRoof(curveArray, plane, level, roofType, 10, 200);
   transaction.Commit();
}

//============代码片段4-13 创建带洞口屋顶============
using (Transaction transaction = new Transaction(RevitDoc))
{
   //创建屋顶前准备参数
   Level level = RevitDoc.GetElement(new ElementId(311)) as Level;
   RoofType roofType = RevitDoc.GetElement(new ElementId(335)) as RoofType;
   CurveArray curveArray = new CurveArray();
   //屋顶外边框
   curveArray.Append(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(30, 0, 0)));
   curveArray.Append(Line.CreateBound(new XYZ(30, 0, 0), new XYZ(30, 30, 0)));
   curveArray.Append(Line.CreateBound(new XYZ(30, 30, 0), new XYZ(0, 30, 0)));
   curveArray.Append(Line.CreateBound(new XYZ(0, 30, 0), new XYZ(0, 0, 0)));
   //在中间添加洞口
   curveArray.Append(Line.CreateBound(new XYZ(5, 5, 0), new XYZ(5, 15, 0)));
   curveArray.Append(Line.CreateBound(new XYZ(5, 15, 0), new XYZ(15, 5, 0)));
   curveArray.Append(Line.CreateBound(new XYZ(15, 5, 0), new XYZ(5, 5, 0)));

   //创建屋顶
   transaction.Start("Create roof");
   ModelCurveArray modelCurveArray = new ModelCurveArray();
   FootPrintRoof roof =
      RevitDoc.Create.NewFootPrintRoof(curveArray, level, roofType, out modelCurveArray);
   //设置屋顶坡度
   ModelCurve curve1 = modelCurveArray.get_Item(0);
   ModelCurve curve3 = modelCurveArray.get_Item(2);
   roof.set_DefinesSlope(curve1, true);
   roof.set_SlopeAngle(curve1, 0.5);
   roof.set_DefinesSlope(curve3, true);
   roof.set_SlopeAngle(curve3, 1.6);
   transaction.Commit();
}

//============代码片段4-14 创建独立实例============
// place a furniture at (0,0,0)
FamilySymbol familySymbol = RevitDoc.GetElement(new ElementId(99774)) as FamilySymbol;
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create standard-alone instance");
   FamilyInstance familyInstance = m_creation.NewFamilyInstance(
      new XYZ(0, 0, 0), familySymbol, StructuralType.NonStructural);
   transaction.Commit();
   Trace.WriteLine(familyInstance.Id);
}

//============代码片段4-15 墙上创建门============
// 在墙上创建一扇门
FamilySymbol familySymbol = RevitDoc.GetElement(new ElementId(49480)) as FamilySymbol;
Level level = RevitDoc.GetElement(new ElementId(30)) as Level;
Wall hostWall = RevitDoc.GetElement(new ElementId(180736)) as Wall;
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create standard-alone instance");
   FamilyInstance familyInstance = m_creation.NewFamilyInstance(
      new XYZ(0, 0, 0), familySymbol, hostWall, level, StructuralType.NonStructural);
   transaction.Commit();
   Trace.WriteLine(familyInstance.Id);
}

//============代码片段4-16 创建柱子============
// place a column at (0,0,0),
FamilySymbol familySymbol = RevitDoc.GetElement(new ElementId(52557)) as FamilySymbol;
Level level = RevitDoc.GetElement(new ElementId(331)) as Level;
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create a level based instance");
   FamilyInstance familyInstance = m_creation.NewFamilyInstance(
      new XYZ(0, 0, 0), familySymbol, level, StructuralType.NonStructural);
   transaction.Commit();
   Trace.WriteLine(familyInstance.Id);
}

//============代码片段4-17 创建线形实例============
// create a beam with an arc
FamilySymbol familySymbol = RevitDoc.GetElement(new ElementId(68912)) as FamilySymbol;
Level level = RevitDoc.GetElement(new ElementId(339)) as Level;
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create standard-alone instance");
   FamilyInstance familyInstance = m_creation.NewFamilyInstance(
      Arc.Create(XYZ.Zero,10,0,2.5,new XYZ(1,0,0),new XYZ(0,1,0)),
      familySymbol, level, StructuralType.Beam);
   transaction.Commit();
   Trace.WriteLine(familyInstance.Id);
}

//============代码片段4-18 创建二维实例============
// 找到标题栏族类型
var titleBlockSymbols = new FilteredElementCollector(RevitDoc).
   OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsElementType();
FamilySymbol titleBlockSymbol = titleBlockSymbols.FirstElement() as FamilySymbol;

// 获取一个ViewSheet
var viewSheet = RevitDoc.GetElement(new ElementId(175782)) as ViewSheet;

// 在ViewSheet里面插入标题栏
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("插入标题栏");

   var familyInstance = m_creation.NewFamilyInstance(XYZ.Zero, titleBlockSymbol, viewSheet);

   transaction.Commit();
}

//============代码片段4-19 创建二维线形实例============
// 获取二维箭头族类型
FamilySymbol arrowSymbol = RevitDoc.GetElement(new ElementId(176135)) as FamilySymbol;

// 找到一个二维视图
var viewPlans = new FilteredElementCollector(RevitDoc).OfClass(typeof(ViewPlan)).OfType<ViewPlan>();
var viewPlan = viewPlans.FirstOrDefault(v => v.GenLevel != null);
var zPosition = viewPlan.GenLevel.Elevation;
// 创建族实例
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("NewFamilyInstance");

   var familyInstance = m_creation.NewFamilyInstance(
      Line.CreateBound(new XYZ(0, 0, zPosition), new XYZ(10, 10, zPosition)),
      arrowSymbol, viewPlan);
   Trace.WriteLine("Created family instance: " + familyInstance.Id);

   transaction.Commit();
}

//============代码片段4-20 创建基于面的族实例============
// 获取类型
FamilySymbol arrowSymbol = RevitDoc.GetElement(new ElementId(574219)) as FamilySymbol;

// 找到墙的一个面
var walls = new FilteredElementCollector(RevitDoc).OfClass(typeof(Wall)).OfType<Wall>();
var wall = walls.FirstOrDefault();
var faceReferences = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);

// 创建实例
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("NewFamilyInstance");

   var familyInstance = m_creation.NewFamilyInstance(faceReferences[0],
      new XYZ(-51.659444225, 5.702513250, 10), new XYZ(0, 1, 1), arrowSymbol);
   Trace.WriteLine("Created family instance: " + familyInstance.Id);

   transaction.Commit();
}

//============代码片段4-21 创建基于面的线形实例============
// 获取类型
FamilySymbol stiffenerSymbol = RevitDoc.GetElement(new ElementId(97916)) as FamilySymbol;

// 找到楼板的一个面
var floors = new FilteredElementCollector(RevitDoc).OfClass(typeof(Floor)).OfType<Floor>();
var floor = floors.FirstOrDefault();
var faceReferences = HostObjectUtils.GetTopFaces(floor);

// 创建实例
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("NewFamilyInstance");

   var familyInstance = m_creation.NewFamilyInstance(
      faceReferences[0],
      Line.CreateBound(new XYZ(36, -40, 0), new XYZ(36, -20, 0)),
      stiffenerSymbol);
   Trace.WriteLine("Created family instance: " + familyInstance.Id);

   transaction.Commit();
}

//============代码片段4-22 批量创建人行道============
// 创建一系列的人行道

// 获取类型
FamilySymbol pavementSymbol = RevitDoc.GetElement(new ElementId(182412)) as FamilySymbol;

// 准备创建数据
List<FamilyInstanceCreationData> list = new List<FamilyInstanceCreationData>();
list.Add(new FamilyInstanceCreationData(new XYZ(10, 0, 0), pavementSymbol, StructuralType.NonStructural));
list.Add(new FamilyInstanceCreationData(new XYZ(20, 0, 0), pavementSymbol, StructuralType.NonStructural));
list.Add(new FamilyInstanceCreationData(new XYZ(30, 0, 0), pavementSymbol, StructuralType.NonStructural));
list.Add(new FamilyInstanceCreationData(new XYZ(40, 0, 0), pavementSymbol, StructuralType.NonStructural));
list.Add(new FamilyInstanceCreationData(new XYZ(50, 0, 0), pavementSymbol, StructuralType.NonStructural));

// 创建实例
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create pavements");
   var familyInstances = m_creation.NewFamilyInstances2(list);
   transaction.Commit();
   foreach (var familyInstanceId in familyInstances)
   {
      Trace.WriteLine(familyInstanceId);
   }
}

//============代码片段4-23 创建房间============
Level level; // 此处省略获取Level的代码
Phase phase; //此处省略获取Phase的代码
// 获取基于标高level的一个视图
var defaultView = new FilteredElementCollector(RevitDoc)
   .WherePasses(new ElementClassFilter(typeof(View)))
   .Cast<View>()
   .Where(v => v.GenLevel != null && v.GenLevel.Id == level.Id)
   .FirstOrDefault();
// 确保视图不为空
if (defaultView != null)
{
   var defaultPhase = defaultView.get_Parameter(BuiltInParameter.VIEW_PHASE);
   if (defaultPhase != null && defaultPhase.AsElementId() == phase.Id)
   {
      using (Transaction transaction = new Transaction(RevitDoc))
      {
         transaction.Start("get_PlanTopology");
         var circuits = RevitDoc.get_PlanTopology(level, phase).Circuits;
         transaction.Commit();
         foreach (PlanCircuit planCircuit in circuits)
         {
            RevitDoc.Create.NewRoom(null, planCircuit);
         }
      }
   }
}

//============代码片段4-24 创建面积============
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create Area");
   //如果在点(30.0, 30.0)这个位置找不到面积边界，将会有警告对话框弹出。
   Area area = RevitDoc.Create.NewArea(areaView, new UV(30.0, 30.0));
   transaction.Commit();
}

//============代码片段4-25 创建四方形闭合区域并在其上创建面积============
using (Transaction transaction = new Transaction(RevitDoc))
{
   var create = RevitDoc.Create;
   //通过创建四条面积边界线来形成一个正方形的闭合区域
   transaction.Start("Create Area Boundary");
   var sketchPlane = areaView.SketchPlane;
   create.NewAreaBoundaryLine(sketchPlane,
      Line.CreateBound(new XYZ(20, 20, 0), new XYZ(40, 20, 0)), areaView);
   create.NewAreaBoundaryLine(sketchPlane,
      Line.CreateBound(new XYZ(40, 20, 0), new XYZ(40, 40, 0)), areaView);
   create.NewAreaBoundaryLine(sketchPlane,
      Line.CreateBound(new XYZ(40, 40, 0), new XYZ(20, 40, 0)), areaView);
   create.NewAreaBoundaryLine(sketchPlane,
      Line.CreateBound(new XYZ(20, 40, 0), new XYZ(20, 20, 0)), areaView);
   transaction.Commit();

   //在新创建的面积边界的中心点(30.0, 30.0)位置放置一个面积
   transaction.Start("Create Area");
   Area area = create.NewArea(areaView, new UV(30.0, 30.0));
   transaction.Commit();
}

//============代码片段4-26 获取拓扑结构============
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("GetPlanTopologies");
   PlanTopologySet planTopologies = RevitDoc.PlanTopologies;
   transaction.Commit();
}

//============代码片段4-27 修改线样式============
ICollection<ElementId> styles = modelCurve.GetLineStyleIds();
foreach (ElementId styleId in styles)
{
   if (styleId != modelCurve.LineStyle.Id)
   {
      using (Transaction transaction = new Transaction(RevitDoc))
      {
         transaction.Start("Create Model Curve");
         modelCurve.LineStyle = RevitDoc.GetElement(styleId) as GraphicsStyle;
         transaction.Commit();
      }
      break;
   }
}

//============代码片段4-28 创建模型线============
using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create Model Line");
   Line geoLine = Line.CreateBound(XYZ.BasisY * 10, XYZ.BasisX * 10);
   SketchPlane modelSketch = SketchPlane.Create(RevitDoc, RevitApp.Create.NewPlane(XYZ.BasisZ, XYZ.Zero));
   ModelCurve modelLine = RevitDoc.Create.NewModelCurve(geoLine, modelSketch);
   transaction.Commit();
}

//============代码片段4-29 创建样条曲线============
using (Transaction transaction = new Transaction(RevitDoc))
{
   SketchPlane modelSketch = SketchPlane.Create(RevitDoc, RevitApp.Create.NewPlane(XYZ.BasisZ, XYZ.Zero));
   transaction.Start("Create Model NurbSpline");
   NurbSpline nurbSpline = NurbSpline.Create(
         new List<XYZ> { new XYZ(0, 0, 0), new XYZ(10, 0, 0), new XYZ(10, 10, 0), new XYZ(20, 10, 0), new XYZ(20, 20, 0) },
         new List<double> { 0.5, 0.1, 0.3, 0.6, 0.8 });
   ModelCurve modelCurve = RevitDoc.Create.NewModelCurve(nurbSpline, modelSketch);
   transaction.Commit();
}

//============代码片段4-30 获取洞口边界============
if (opening.IsRectBoundary)
{
   XYZ startPoint = opening.BoundaryRect[0];
   XYZ endPoint = opening.BoundaryRect[1];
} else {
   foreach (Curve curve in opening.BoundaryCurves)
   {
      //遍历Curve
   }
}

//============代码片段4-31 在墙上开洞口============
Wall wall = GetElement<Wall>(185520);
LocationCurve locationCurve = wall.Location as LocationCurve;
Line location = locationCurve.Curve as Line;
XYZ startPoint = location.get_EndPoint(0);
XYZ endPoint = location.get_EndPoint(1);
Parameter wallHeightParameter = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
double wallHeight = wallHeightParameter.AsDouble();
XYZ delta = (endPoint - startPoint + new XYZ(0, 0, wallHeight)) / 3;

using (Transaction transaction = new Transaction(RevitDoc))
{
   transaction.Start("Create Opening on wall");
   Opening opening = RevitDoc.Create.NewOpening(wall, startPoint + delta, startPoint + delta * 2);
   transaction.Commit();
}

//============代码片段5-1：创建线性尺寸标注============
Transaction transaction = new Transaction(m_revit.Application.ActiveUIDocument.Document, "添加标注");
transaction.Start();
//取得墙，给墙的两个端点创建线性尺寸
for (int i = 0; i < m_walls.Count; i++)
{
  Wall wallTemp = m_walls[i] as Wall;
  if (null == wallTemp)
  {
  continue;
 }

 //取得位置线
 Location location = wallTemp.Location;
 LocationCurve locationline = location as LocationCurve;
 if (null == locationline)
 {
  continue;
 }

 Line newLine = null;

 //取得参考
 ReferenceArray referenceArray = new ReferenceArray();

 AnalyticalModel analyticalModel = wallTemp.GetAnalyticalModel();
 IList<Curve> activeCurveList  = analyticalModel.GetCurves(AnalyticalCurveType.ActiveCurves);
 foreach (Curve aCurve in activeCurveList)
 {
  // 从分析模型中找到不垂直的线
  if (aCurve.GetEndPoint(0).Z == aCurve.GetEndPoint(1).Z)
     newLine = aCurve as Line;
  if (aCurve.GetEndPoint(0).Z != aCurve.GetEndPoint(1).Z)
  {
     AnalyticalModelSelector amSelector = new AnalyticalModelSelector(aCurve);
     amSelector.CurveSelector = AnalyticalCurveSelector.StartPoint;
     referenceArray.Append(analyticalModel.GetReference(amSelector));
  }
  if (2 == referenceArray.Size)
     break;
 }
 if (referenceArray.Size != 2)
 {
  m_errorMessage += "Did not find two references";
  return false;
 }
 try
 {
  //创建尺寸
  Autodesk.Revit.UI.UIApplication app = m_revit.Application;
  Document doc = app.ActiveUIDocument.Document;

  Autodesk.Revit.DB.XYZ p1 = new XYZ(
     newLine.GetEndPoint(0).X + 5,
     newLine.GetEndPoint(0).Y + 5,
     newLine.GetEndPoint(0).Z);
  Autodesk.Revit.DB.XYZ p2 = new XYZ(
     newLine.GetEndPoint(1).X + 5,
     newLine.GetEndPoint(1).Y + 5,
     newLine.GetEndPoint(1).Z);

  Line newLine2 = Line.CreateBound(p1, p2);
  Dimension newDimension = doc.Create.NewDimension(
     doc.ActiveView, newLine2, referenceArray);
 }
 catch (Exception ex)
 {
  m_errorMessage += ex.ToString();
  return false;
 }
}
transaction.Commit();

//============代码片段6-1：几何============
         public Options GetGeometryOption()
{
              Autodesk.Revit.DB.Options option = this.Application.Create.NewGeometryOptions();
   option.ComputeReferences = true;   //打开计算几何引用
   option.DetailLevel = ViewDetailLevel.Fine;   //视图详细程度为最好
              return option;
 }

//============代码片段6-2：几何============
public void GetWallGeometry()
{
    Document doc = this.ActiveUIDocument.Document;        
    Wall aWall = doc.GetElement(new ElementId(186388)) as Wall;
            
    Options option = GetGeometryOption();  // 创建几何选项
Autodesk.Revit.DB.GeometryElement geomElement = aWall.get_Geometry(option);
foreach (GeometryObject geomObj in geomElement)
{
   Solid geomSolid = geomObj as Solid;
   if (null != geomSolid)
   {
      foreach (Face geomFace in geomSolid.Faces)
      {
         // 得到墙的面
      }
      foreach (Edge geomEdge in geomSolid.Edges)
      {
         // 得到墙的边
      }
   }
}
}

//============代码片段6-3：几何============
public void GetInstanceGeometry_Curve()
{
Document doc = this.ActiveUIDocument.Document;    
FamilyInstance familyInstance = doc.GetElement(new ElementId(187032)) as FamilyInstance;
Options option = GetGeometryOption();
Autodesk.Revit.DB.GeometryElement geomElement = familyInstance.get_Geometry(option);

foreach (GeometryObject geomObj in geomElement)
{
   GeometryInstance geomInstance = geomObj as GeometryInstance;
   if (null != geomInstance)
   {
       foreach (GeometryObject instObj in geomInstance.SymbolGeometry)
       {
          Curve curve = instObj as Curve;
          if (null != curve)
          {
              // 把取到的线变换到实例的坐标系中
              curve = curve.CreateTransformed(geomInstance.Transform);
              // …
          }
        }
    }
 }
}

//============代码片段6-4：几何============
public void GetInstanceGeometry_Solid()
{
    Document doc = this.ActiveUIDocument.Document;
    FamilyInstance familyInstance = doc.GetElement(new ElementId(187758)) as FamilyInstance;
    Options option = GetGeometryOption();
    Autodesk.Revit.DB.GeometryElement geomElement = familyInstance.get_Geometry(option);

    foreach (GeometryObject geomObj in geomElement)
    {
       GeometryInstance geomInstance = geomObj as GeometryInstance;
       if (null != geomInstance)
       {
          foreach (GeometryObject instObj in geomInstance.SymbolGeometry)
          {
             Solid solid = instObj as Solid;
             if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size)
             {
                continue;
             }
             Transform instTransform = geomInstance.Transform;
             // 从实体Solid获取面和边，然后对点进行变换
             foreach (Face face in solid.Faces)
             {
                Mesh mesh = face.Triangulate();
                foreach (XYZ ii in mesh.Vertices)
                {
                   XYZ point = ii;
                   XYZ transformedPoint = instTransform.OfPoint(point);
                }
             }
             foreach (Edge edge in solid.Edges)
             {
                foreach (XYZ ii in edge.Tessellate())
                {
                   XYZ point = ii;
                   XYZ transformedPoint = instTransform.OfPoint(point);
                }
             }
          }
      }
    }
 }

//============代码片段6-5：几何============
public void DrawMesh()
{
    Document doc = this.ActiveUIDocument.Document;
    Transaction transaction = new Transaction(doc, "Draw Mesh");
    transaction.Start();

    Sweep sweep = doc.GetElement(new ElementId(2311)) as Sweep;
    Options option = GetGeometryOption();
    Autodesk.Revit.DB.GeometryElement geomElement = sweep.get_Geometry(option);

    foreach (GeometryObject geomObj in geomElement)
    {
       Solid geomSolid = geomObj as Solid;
       if (null != geomSolid)
       {
          foreach (Face geomFace in geomSolid.Faces)
          {
             // 对面进行三角面片化形成一个网格
             Mesh mesh = geomFace.Triangulate();
             for (int i = 0; i < mesh.NumTriangles; i++)
             {
                MeshTriangle triangular = mesh.get_Triangle(i);
                // 定义 XYZ 列表来存放三角形的顶点
                List<XYZ> triangularPoints = new List<XYZ>();
                for (int n = 0; n < 3; n++)
                {
                   XYZ point = triangular.get_Vertex(n);
                   triangularPoints.Add(point);
                }
                // 调用方法把所有三角形在文档中描绘出来
                DrawTriangle(doc, triangularPoints);
             }
          }
       }
    }

    transaction.Commit();
 }

//============代码片段6-6：几何============
// GeometryObject geoObj = GetGeometryObject();
Solid solid = geoObj as Solid;
if(null != solid && 0 != solid.Faces.Size)
{
// 先判断再使用
}

//============代码片段6-7：几何============
 public static XYZ TransformPoint(XYZ point, Transform transform)
 {
    double x = point.X;
    double y = point.Y;
    double z = point.Z;
    //获取变换的原点和基向量
    XYZ b0 = transform.get_Basis(0);
    XYZ b1 = transform.get_Basis(1);
    XYZ b2 = transform.get_Basis(2);
    XYZ origin = transform.Origin;
    //对原来坐标系统的点在新的坐标系统进行变换
    double xTemp = x * b0.X + y * b1.X + z * b2.X + origin.X;
    double yTemp = x * b0.Y + y * b1.Y + z * b2.Y + origin.Y;
    double zTemp = x * b0.Z + y * b1.Z + z * b2.Z + origin.Z;
    return new XYZ(xTemp, yTemp, zTemp);
 }

//============代码片段6-8：几何============
  // ReferencePlane refPlane = GetRefPlane();
 Transform mirTrans = Transform.CreateReflection(refPlane.Plane);

//============代码片段6-9：几何============
Autodesk.Revit.DB.Options option = this.Application.Create.NewGeometryOptions();
option.ComputeReferences = true;
option.DetailLevel = ViewDetailLevel.Medium;

//============代码片段6-10：几何============
public void ModifySectionBox()
{        
  Document doc = this.ActiveUIDocument.Document;    
  using(Transaction transaction = new Transaction(doc, "Modify Section Box"))
  {
    transaction.Start();
                
    View3D view3d = doc.GetElement(new ElementId(186350)) as View3D;            
    BoundingBoxXYZ box = view3d.GetSectionBox();
    if (false == box.Enabled)
    {
      TaskDialog.Show("Error", "The section box for View3D isn't Enable.");
      return;
    }
    // 创建旋转变换
    XYZ origin = new XYZ(0, 0, 0);
    XYZ axis = new XYZ(0, 0, 1);
    Transform rotate = Transform.CreateRotationAtPoint(axis, 2, origin);
    // 把旋转变换应用于三维视图的剖面框
    box.Transform = box.Transform.Multiply(rotate);
    view3d.SetSectionBox(box);
                
    transaction.Commit();
  }
}

//============代码片段6-11：几何============
Document projectDoc = ActiveUIDocument.Document;
View3D view = projectDoc.ActiveView as View3D;
if(view != null)
{
   BoundingBoxUV bbBoxUV = view.Outline;
   UV max = bbBoxUV.Max;
   UV min = bbBoxUV.Min;
}

//============代码片段6-12：几何============
//[in]geomElement, [out]curves, [out]solids
public void GetCurvesFromABeam(FamilyInstance beam, Options options, CurveArray curves, SolidArray solids)
{
  Autodesk.Revit.DB.GeometryElement geomElement = beam.get_Geometry(options);
  //找到所有的实体solids和线curves
  AddCurvesAndSolids(geomElement, curves, solids);
}

private void AddCurvesAndSolids(GeometryElement geomElem, CurveArray curves, SolidArray solids)
{
  foreach (GeometryObject geomObject in geomElem)
  {
    Curve curve = geomObject  as Curve;
    if (null != curve)
    {
      curves.Append(curve);
      continue;
    }
    Solid solid = geomObject  as Solid;
    if (null != solid)
    {
      solids.Append(solid);
      continue;
    }
    //如果GeometryObject 是几何实例，则进行二次遍历
    GeometryInstance geomInst = geomObject  as GeometryInstance;
    if (null != geomInst)
    {
      AddCurvesAndSolids(geomInst.GetSymbolGeometry(geomInst.Transform), curves, solids);
    }
  }
}

//============代码片段7-1 获取族管理器============
Document doc;  // 得到族文档
if(doc.IsFamilyDocument)
{
   // 只有当IsFamilyDocument等于true， FamilyManager才能得到
   FamilyManager familyMgr = doc.FamilyManager;
}

//============代码片段7-2 获取当前族类型============
//得到FamilyManager
FamilyManager familyMgr = doc.FamilyManager;
FamilyType currentType = familyMgr.CurrentType;
if(currentType != null)
{
    //当前族类型可能为空，建议在其他操作前加上空检查
}

//============代码片段7-3 创建新的类型============
// 得到FamilyManager
FamilyManager familyMgr = doc.FamilyManager;
// 族类型的名字，保证在所有族类型中唯一
string newTypeName = "UniqueName";
// 创建，得到FamilyType的实例
FamilyType famType = familyMgr.NewType(newTypeName);

//============代码片段7-4 删除族类型============
// 得到FamilyManager
FamilyManager familyMgr = doc.FamilyManager;
if(familyMgr.CurrentType != null)
{
    // 只有当前族类型存在，我们才能调用下面的删除方法
    familyMgr.DeleteCurrentType();
    // 一般来说，当删除结束后，第一个族类型会成为当前类型，
    // 但是为了确保安全, 建议你显式设置成你需要的类型
    if (familyMgr.Types.Size != 0)
    {
        FamilyType type = familyMgr.Types.Cast<FamilyType>().ElementAt(0);
        familyMgr.CurrentType = type;
    }
}

//============代码片段7-5 族类型重命名============
// 得到FamilyManager
FamilyManager familyMgr = doc.FamilyManager;
// 族类型的名字，保证在所有族类型中唯一
string newTypeName = "UniqueName";
familyMgr.RenameCurrentType(newTypeName);

//============代码片段7-6 创建共享族参数============
Autodesk.Revit.ApplicationServices.Application app; // 得到Application
// 得到FamilyManager
FamilyManager familyMgr = doc.FamilyManager;

// 共享参数的基本信息, 包括定义文件路径，参数分组名称，参数名称和参数类型。
string sharedParameterFilePath = @"C:\SharedParameter.txt";
string sharedParameterGroupName = "Shared_Group";
string sharedParameter = "Shared_Parameter";
ParameterType sharedParameterType = ParameterType.Length;

// 打开或创建共享参数定义文件。
app.SharedParametersFilename = sharedParameterFilePath;
DefinitionFile sharedDefinitonFile = app.OpenSharedParameterFile();
if (sharedDefinitonFile == null)
   return;

// 查找共享参数的分组名称，如果没找到，就创建一个。
DefinitionGroup sharedGroup = null;
sharedGroup = sharedDefinitonFile.Groups.get_Item(sharedParameterGroupName);
if (null == sharedGroup)
   sharedGroup = sharedDefinitonFile.Groups.Create(sharedParameterGroupName);

// 查找共享参数的定义，如果没有找到，就用名字和类型创建一个。
ExternalDefinition parameterDef = sharedGroup.Definitions.get_Item(sharedParameter) as ExternalDefinition;
if (null == parameterDef)
   parameterDef = sharedGroup.Definitions.Create(sharedParameter, sharedParameterType) as ExternalDefinition;

// 创建共享族参数
FamilyParameter newParameter = familyMgr.AddParameter(parameterDef, BuiltInParameterGroup.PG_CONSTRAINTS, true);

//============代码片段7-7 创建一般族参数============
// 得到FamilyManager
FamilyManager familyMgr = doc.FamilyManager;
string paraName = "UniqueName"; // 唯一
// 设置族参数的类别和类型
BuiltInParameterGroup paraGroup = BuiltInParameterGroup.PG_LENGTH;
ParameterType paraType = ParameterType.Length;
// 设置族参数为实例参数
bool isInstance = true;
// 创建族参数
FamilyParameter newParameter = familyMgr.AddParameter(paraName, paraGroup, paraType, isInstance);

//============代码片段7-8 创建族类型参数============
// 得到FamilyManager
FamilyManager familyMgr = doc.FamilyManager;
string paraName = "族类型参数"; // 唯一
BuiltInParameterGroup paraGroup = BuiltInParameterGroup.PG_TEXT;
// 设置族参数为实例参数
bool isInstance = true;

// 得到一个已加载的族类型（FamilySymbol）的组。
FilteredElementCollector cotr = new FilteredElementCollector(doc);
IList<Element> symbols = cotr.OfClass(typeof(FamilySymbol)).ToElements();
if (symbols.Count == 0)
   return;   // 未找到族类型，跳过创建
Category famCategory = symbols[0].Category;

// 创建族参数
FamilyParameter newParameter = familyMgr.AddParameter(paraName, paraGroup, famCategory, isInstance);

//============代码片段7-9 设置族参数公式============
/// <summary>
/// 这个方法演示如何创建族参数的公式。
/// 包括了如下几种类型的族参数：长度，面积，体积和字符串类型。
/// </summary>
public void SetParameterFormula(
    FamilyManager familyMgr, FamilyParameter lengthPara,
    FamilyParameter areaPara, FamilyParameter volumePara,
    FamilyParameter commentPara)
{
    // 字符串类型的族参数的公式，必须要带上""
    familyMgr.SetFormula(commentPara, "\"注释内容\"");

    // 有单位的族参数（例如：长度类型）的公式，应该带上单位
    familyMgr.SetFormula(lengthPara, "1000mm");

    // 族参数的公式需要符合其类型的定义
    // 例如面积类型的参数可以是2个长度参数的乘积
    string lengtthParaName = lengthPara.Definition.Name;
    familyMgr.SetFormula(areaPara, lengtthParaName + " * " + lengtthParaName);

    // 而体积类型的参数可以是3个长度参数的乘积
    familyMgr.SetFormula(volumePara, lengtthParaName
        + " * " + lengtthParaName + " * " + lengtthParaName);
}

//============代码片段7-10 设置族参数的值============
public void SetFamilyParameterValue(
    FamilyManager familyMgr, FamilyParameter lengthPara,
    FamilyParameter commentPara, FamilyParameter IntPara,
    FamilyParameter materialPara, ElementId newMaterialId)
{
    // 首先需要判断当前族类型是否存在，如果不存在，读写族参数都是不可行的
    FamilyType currentType = familyMgr.CurrentType;
    if (null == currentType)
        return;

    // 读写长度类型的族参数；他的储存类型为double
    double length = currentType.AsDouble(lengthPara).Value;
    familyMgr.Set(lengthPara, length + 1.25);

    // 读写文字类型的族参数；他的储存类型为string
    string comment = currentType.AsString(commentPara);
    familyMgr.Set(commentPara, "新的注释");

    // 读写整数类型的族参数；他的储存类型为int
    int integerVale = currentType.AsInteger(IntPara).Value;
    familyMgr.Set(IntPara, integerVale + 3);

    // 读写材质类型的族参数；他的储存类型为ElementId
    ElementId materialId = currentType.AsElementId(materialPara);
    familyMgr.Set(materialPara, newMaterialId);

    // 您可以读写族参数的可视化文字表示的值
    string lengthText = currentType.AsValueString(lengthPara);
    familyMgr.SetValueString(lengthPara, "10m");
}

//============代码片段7-11 关联族参数============
public void AssociateParametersInFamilyDocument(FamilyManager familyMgr,
    Parameter lengthElementPara,        // 长度类型的图元参数
    FamilyParameter lengthFamilyPara    // 长度类型的族参数
    )
{
    ParameterType lengthElementParaType = lengthElementPara.Definition.ParameterType;
    ParameterType lengthFamilyParaType = lengthFamilyPara.Definition.ParameterType;

    // 能关联起来的图元参数和族参数的类型要相同
    if (lengthElementParaType != lengthFamilyParaType)
    {
        // 注意：有些类型不同的图元参数和族参数也是可以关联的。
        // 比如PipeSize和Length类型的参数可以关联，因为他们代表了相同的物理量：长度
        return;
    }

    // 判断该长度单位是否能够和族参数关联
    if (!familyMgr.CanElementParameterBeAssociated(lengthElementPara))
        return;

    // 进行关联操作
    familyMgr.AssociateElementParameterToFamilyParameter(lengthElementPara, lengthFamilyPara);
}

//============代码片段7-12 关联族参数和尺寸标注============
public void AssociateParameterToDimension(
    Dimension dimension, FamilyParameter familyPara)
{
    // 获取当前关联的族参数
    FamilyParameter currentPara = dimension.FamilyLabel;
    if(null != currentPara)
    {
        // 新关联的族参数类型应该和现有的族参数类型一致。
        // 就像上个例子提到的一样，他们至少应该是物理意义相同。
        if (currentPara.Definition.ParameterType != familyPara.Definition.ParameterType)
            return;
    }

    // 关联新的族参数
    dimension.FamilyLabel = familyPara;

    // 清空关联
    dimension.FamilyLabel = null;
}

//============代码片段7-13 创建拉伸体============
private Application m_revit; // 已获得Revit的Application的实例
private Document m_familyDocument;  // 已获得族文档的实例
// 创建工作平面的函数，输入为平面的原点和法向量
internal SketchPlane CreateSketchPlane(Autodesk.Revit.DB.XYZ normal, Autodesk.Revit.DB.XYZ origin)
{
   // 首先创建几何平面
   Plane geometryPlane = m_revit.Create.NewPlane(normal, origin);
   if (null == geometryPlane)
   {
return null;
   }
   // 根绝几何平面创建工作平面
   SketchPlane plane = SketchPlane.Create(m_familyDocument, geometryPlane);
   if (null == plane)
   {
      return null;
   }
   return plane;
}

// 创建用于拉伸的轮廓线
private CurveArrArray CreateExtrusionProfile()
{
   // 轮廓线可以包括一个或者多个闭合的轮廓，所以最后返回是CurveArrArray
   CurveArrArray curveArrArray = new CurveArrArray();
   CurveArray curveArray1 = new CurveArray();

   // 创建一个正方体的轮廓线，先创建点，再创建线，最后组合成轮廓。
   Autodesk.Revit.DB.XYZ p0 = Autodesk.Revit.DB.XYZ.Zero;
   Autodesk.Revit.DB.XYZ p1 = new Autodesk.Revit.DB.XYZ(10, 0, 0);
   Autodesk.Revit.DB.XYZ p2 = new Autodesk.Revit.DB.XYZ(10, 10, 0);
   Autodesk.Revit.DB.XYZ p3 = new Autodesk.Revit.DB.XYZ(0, 10, 0);
   Line line1 = Line.CreateBound(p0, p1);
   Line line2 = Line.CreateBound(p1, p2);
   Line line3 = Line.CreateBound(p2, p3);
   Line line4 = Line.CreateBound(p3, p0);
   curveArray1.Append(line1);
   curveArray1.Append(line2);
   curveArray1.Append(line3);
   curveArray1.Append(line4);
   curveArrArray.Append(curveArray1);
   return curveArrArray;
}

private void CreateExtrusion(FamilyItemFactory familyCreator)
{
   // 调用函数创建拉伸的轮廓线和工作平面
   CurveArrArray curveArrArray = CreateExtrusionProfile();
   SketchPlane sketchPlane = CreateSketchPlane(XYZ.BasisZ, XYZ.Zero);

   // 调用API创建拉伸（实心正方体）
   Extrusion rectExtrusion = familyCreator.NewExtrusion(true, curveArrArray, sketchPlane, 10)
   // 可能我们会希望把拉伸体移动到希望的位置上
   XYZ transPoint1 = new XYZ(-16, 0, 0);
   ElementTransformUtils.MoveElement(m_familyDocument, rectExtrusion.Id, transPoint1);
}

//============代码片段7-14 创建融合体============
private void CreateBlend(FamilyItemFactory familyCreator)
{
    CurveArray topProfile = new CurveArray();
    CurveArray baseProfile = new CurveArray();

    // 创建工作平面，CreateSketchPlane见上一个例子。
    Autodesk.Revit.DB.XYZ normal = Autodesk.Revit.DB.XYZ.BasisZ;
    SketchPlane sketchPlane = CreateSketchPlane(normal, Autodesk.Revit.DB.XYZ.Zero);

    // 创建底部的闭合二维轮廓
    Autodesk.Revit.DB.XYZ p00 = Autodesk.Revit.DB.XYZ.Zero;
    Autodesk.Revit.DB.XYZ p01 = new Autodesk.Revit.DB.XYZ(10, 0, 0);
    Autodesk.Revit.DB.XYZ p02 = new Autodesk.Revit.DB.XYZ(10, 10, 0);
    Autodesk.Revit.DB.XYZ p03 = new Autodesk.Revit.DB.XYZ(0, 10, 0);
    Line line01 = Line.CreateBound(p00, p01);
    Line line02 = Line.CreateBound(p01, p02);
    Line line03 = Line.CreateBound(p02, p03);
    Line line04 = Line.CreateBound(p03, p00);
    baseProfile.Append(line01);
    baseProfile.Append(line02);
    baseProfile.Append(line03);
    baseProfile.Append(line04);

    // 创建顶部的闭合二维轮廓
    Autodesk.Revit.DB.XYZ p10 = new Autodesk.Revit.DB.XYZ(5, 2, 10);
    Autodesk.Revit.DB.XYZ p11 = new Autodesk.Revit.DB.XYZ(8, 5, 10);
    Autodesk.Revit.DB.XYZ p12 = new Autodesk.Revit.DB.XYZ(5, 8, 10);
    Autodesk.Revit.DB.XYZ p13 = new Autodesk.Revit.DB.XYZ(2, 5, 10);
    Line line11 = Line.CreateBound(p10, p11);
    Line line12 = Line.CreateBound(p11, p12);
    Line line13 = Line.CreateBound(p12, p13);
    Line line14 = Line.CreateBound(p13, p10);
    topProfile.Append(line11);
    topProfile.Append(line12);
    topProfile.Append(line13);
    topProfile.Append(line14);

    // 利用底部和顶部的轮廓来创建融合
    Blend blend = familyCreator.NewBlend(true, topProfile, baseProfile, sketchPlane);
}

//============代码片段7-15 编辑融合体============
public void AccessBlend(Blend blend)
{
    // 得到融合体的底部和顶部轮廓线
    CurveArrArray baseProfile = blend.BottomProfile;
    CurveArrArray topProfile = blend.TopProfile;

    // 通过修改TopOffset属性来让融合体的顶部更高
    blend.TopOffset = blend.TopOffset + 1;

    // 修改融合体顶部和底部的映射关系
    VertexIndexPairArray currentMap = blend.GetVertexConnectionMap();
    VertexIndexPairArray newMap = new VertexIndexPairArray();
    List<int> topIndexes = new List<int>();
    List<int> bottomIndexes = new List<int>();
    foreach(VertexIndexPair pair in currentMap)
    {
        topIndexes.Add(pair.Top);
        bottomIndexes.Add(pair.Bottom);
    }
    int lastTopindex = topIndexes.Last();
    topIndexes.RemoveAt(topIndexes.Count - 1);
    topIndexes.Insert(0, lastTopindex);
    for(int idx = 0; idx < bottomIndexes.Count; idx++)
    {
        newMap.Append(new VertexIndexPair(topIndexes[idx], bottomIndexes[idx]));
    }
    blend.SetVertexConnectionMap(newMap);
}

//============代码片段7-16 创建旋转体============
private void CreateRevolution(FamilyItemFactory familyCreator)
{
    CurveArrArray curveArrArray = new CurveArrArray();
    CurveArray curveArray = new CurveArray();

    // 创建工作平面，CreateSketchPlane参见Extrusion中的例子。
    Autodesk.Revit.DB.XYZ normal = Autodesk.Revit.DB.XYZ.BasisZ;
    SketchPlane sketchPlane = CreateSketchPlane(normal, Autodesk.Revit.DB.XYZ.Zero);

    // 创建一个正方体的轮廓线
    Autodesk.Revit.DB.XYZ p0 = Autodesk.Revit.DB.XYZ.Zero;
    Autodesk.Revit.DB.XYZ p1 = new Autodesk.Revit.DB.XYZ(10, 0, 0);
    Autodesk.Revit.DB.XYZ p2 = new Autodesk.Revit.DB.XYZ(10, 10, 0);
    Autodesk.Revit.DB.XYZ p3 = new Autodesk.Revit.DB.XYZ(0, 10, 0);
    Line line1 = Line.CreateBound(p0, p1);
    Line line2 = Line.CreateBound(p1, p2);
    Line line3 = Line.CreateBound(p2, p3);
    Line line4 = Line.CreateBound(p3, p0);
    curveArray.Append(line1);
    curveArray.Append(line2);
    curveArray.Append(line3);
    curveArray.Append(line4);

// NewRevolution函数需要一个轮廓线的集合来满足复杂的轮廓线，比如“回”字形。
curveArrArray.Append(curveArray);

// 创建旋转轴
    Autodesk.Revit.DB.XYZ pp = new Autodesk.Revit.DB.XYZ(1, -1, 0);
    Line axis1 = Line.CreateBound(Autodesk.Revit.DB.XYZ.Zero, pp);

    // 利用轮廓线和旋转轴来创建旋转体
    Revolution revolution1 = familyCreator.NewRevolution(true, curveArrArray, sketchPlane, axis1, -Math.PI, 0);
}

//============代码片段7-17 创建放样体============
private void CreateSweep(Autodesk.Revit.Creation.Application appCreator,
    FamilyItemFactory familyCreator)
{
    CurveArrArray arrarr = new CurveArrArray();
    CurveArray arr = new CurveArray();

    // 创建路径平面，CreateSketchPlane参见Extrusion中的例子。
    Autodesk.Revit.DB.XYZ normal = Autodesk.Revit.DB.XYZ.BasisZ;
    SketchPlane sketchPlane = CreateSketchPlane(normal, Autodesk.Revit.DB.XYZ.Zero);

// 创建用于放样的轮廓，这里创建轮廓线再生成轮廓的方式
    Autodesk.Revit.DB.XYZ pnt1 = new Autodesk.Revit.DB.XYZ(0, 0, 0);
    Autodesk.Revit.DB.XYZ pnt2 = new Autodesk.Revit.DB.XYZ(2, 0, 0);
    Autodesk.Revit.DB.XYZ pnt3 = new Autodesk.Revit.DB.XYZ(1, 1, 0);
    arr.Append(Arc.Create(pnt2, 1.0d, 0.0d, 180.0d, Autodesk.Revit.DB.XYZ.BasisX, Autodesk.Revit.DB.XYZ.BasisY));
    arr.Append(Arc.Create(pnt1, pnt3, pnt2));
    arrarr.Append(arr);
    SweepProfile profile = appCreator.NewCurveLoopsProfile(arrarr);

    // 创建用于放样的路径，该路径包括2条线段
    Autodesk.Revit.DB.XYZ pnt4 = new Autodesk.Revit.DB.XYZ(10, 0, 0);
    Autodesk.Revit.DB.XYZ pnt5 = new Autodesk.Revit.DB.XYZ(0, 10, 0);
    Autodesk.Revit.DB.XYZ pnt6 = new Autodesk.Revit.DB.XYZ(5, 13, 0);
    Curve curve = Line.CreateBound(pnt4, pnt5);
    Curve curve1 = Line.CreateBound(pnt5, pnt6);
    CurveArray curves = new CurveArray();
    curves.Append(curve);
    curves.Append(curve1);

    // 利用轮廓和拉伸路径来创建放样，轮廓线位于拉伸路径的第二条线段的中心点
    Sweep sweep1 = familyCreator.NewSweep(true, curves, sketchPlane, profile, 1, ProfilePlaneLocation.MidPoint);
}

//============代码片段7-18 创建放样融合体============
private void CreateSweepBlend(Autodesk.Revit.Creation.Application appCreator,
    FamilyItemFactory familyCreator)
{
    // 创建底部的轮廓线
    Autodesk.Revit.DB.XYZ pnt1 = new Autodesk.Revit.DB.XYZ(0, 0, 0);
    Autodesk.Revit.DB.XYZ pnt2 = new Autodesk.Revit.DB.XYZ(1, 0, 0);
    Autodesk.Revit.DB.XYZ pnt3 = new Autodesk.Revit.DB.XYZ(1, 1, 0);
    Autodesk.Revit.DB.XYZ pnt4 = new Autodesk.Revit.DB.XYZ(0, 1, 0);
    Autodesk.Revit.DB.XYZ pnt5 = new Autodesk.Revit.DB.XYZ(0, 0, 1);
    CurveArrArray arrarr1 = new CurveArrArray();
    CurveArray arr1 = new CurveArray();
    arr1.Append(Line.CreateBound(pnt1, pnt2));
    arr1.Append(Line.CreateBound(pnt2, pnt3));
    arr1.Append(Line.CreateBound(pnt3, pnt4));
    arr1.Append(Line.CreateBound(pnt4, pnt1));
    arrarr1.Append(arr1);

    // 创建顶部的轮廓线
    Autodesk.Revit.DB.XYZ pnt6 = new Autodesk.Revit.DB.XYZ(0.5, 0, 0);
    Autodesk.Revit.DB.XYZ pnt7 = new Autodesk.Revit.DB.XYZ(1, 0.5, 0);
    Autodesk.Revit.DB.XYZ pnt8 = new Autodesk.Revit.DB.XYZ(0.5, 1, 0);
    Autodesk.Revit.DB.XYZ pnt9 = new Autodesk.Revit.DB.XYZ(0, 0.5, 0);
    CurveArrArray arrarr2 = new CurveArrArray();
    CurveArray arr2 = new CurveArray();
    arr2.Append(Line.CreateBound(pnt6, pnt7));
    arr2.Append(Line.CreateBound(pnt7, pnt8));
    arr2.Append(Line.CreateBound(pnt8, pnt9));
    arr2.Append(Line.CreateBound(pnt9, pnt6));
    arrarr2.Append(arr2);

    // 利用底部和顶部的轮廓线生成底部和顶部的轮廓。
    SweepProfile bottomProfile = appCreator.NewCurveLoopsProfile(arrarr1);
    SweepProfile topProfile = appCreator.NewCurveLoopsProfile(arrarr2);

    // 创建放样融合的拉伸路径
    Autodesk.Revit.DB.XYZ pnt10 = new Autodesk.Revit.DB.XYZ(5, 0, 0);
    Autodesk.Revit.DB.XYZ pnt11 = new Autodesk.Revit.DB.XYZ(0, 20, 0);
    Curve curve = Line.CreateBound(pnt10, pnt11);

// 创建路径平面，CreateSketchPlane参见Extrusion中的例子。
    Autodesk.Revit.DB.XYZ normal = Autodesk.Revit.DB.XYZ.BasisZ;
    SketchPlane sketchPlane = CreateSketchPlane(normal, Autodesk.Revit.DB.XYZ.Zero);

    // 利用底部和顶部的轮廓和拉伸路径创建放样融合
    SweptBlend newSweptBlend1 = familyCreator.NewSweptBlend(true, curve, sketchPlane, bottomProfile, topProfile);
}

//============代码片段7-19 管理图元可见性============
public void AccessFamilyElementVisibility(Extrusion extrusion)
{
    // 得到管理拉伸体的可见性的实例，并读取详细程度的设置
    FamilyElementVisibility visibility = extrusion.GetVisibility();
    FamilyElementVisibilityType visibilityType = visibility.VisibilityType;
    bool shownInCoarse = visibility.IsShownInCoarse;
    bool shownInMedium = visibility.IsShownInMedium;
    bool shownInFine = visibility.IsShownInFine;

    // 设置为在各种详细程度中都显示拉伸体
    visibility.IsShownInCoarse = true;
    visibility.IsShownInMedium = true;
visibility.IsShownInFine = true;
// 注意：必须把可见性的修改设置回拉伸体
    extrusion.SetVisibility(visibility);
}

//============代码片段8-1：获取视图类型============
//Autodesk.Revit.DB.View view = GetView();
//两种判断视图类型的方法：
//第一种：
ViewType viewType = view.ViewType;
switch (viewType)
{
 case Autodesk.Revit.DB.ViewType.ThreeD:
  // 视图类型是三维视图
  break;
 // 其他类型
}

// 第二种：
if (view is View3D)
{
 // view的类类型是三维视图
}

//============代码片段8-2：获取视图中可见的元素============
//找到视图中所有可见的元素
FilteredElementCollector elemCollector = new FilteredElementCollector(document, viewId);
foreach (Element elem in elemCollector)
{
    //操作元素elem
}

//============代码片段8-3：创建一个正交三维视图============
private void CreateView3D(Autodesk.Revit.Document doc, ElementId viewTypeId)
{
  try
  {
   // Create a new View3D
   XYZ direction = new XYZ(1, 1, 1);
   View3D view3D = View3D.CreateIsometric(doc, viewTypeId);
   if (null == view3D)
    return;

   // The created View3D isn't perspective.
   Debug.Assert(false == view3D.IsPerspective);
  }
  catch (Exception e)
  {
   Debug.WriteLine(e.ToString());
  }
}

//============代码片段8-4：显示剖面框============
private void ShowHideSectionBox(View3D view3D)
{
  view3D.IsSectionBoxActive = true;
}

//============代码片段8-5：创建楼层平面和天花板平面============
private void CreatePlanView(Autodesk.Revit.Document doc, ElementId viewTypeId)
{
  try
  {
   using(Transaction tr = new Transaction(doc))
   {
    tr.Start(“创建平面视图”);
    double elevation = 10.0;
    Level level1 = doc.Create.NewLevel(elevation);
    ViewPlan floorView = ViewPlan.Create(doc, viewTypeId, level1.Id);
    tr.Commit();
   }
  }
  catch (Exception exp)
  {
   MessageBox.Show(exp.ToString());
  }
}

//============代码片段8-6：创建和打印一个图纸视图============
private void CreateSheetView(Autodesk.Revit.Document doc)
{
  // Get an available title block from document
  FamilySymbolSet fsSet = doc.TitleBlocks;
  if (fsSet.Size == 0)
  {
   MessageBox.Show("No title blocks");
   return;
  }

  FamilySymbol fs = null;
  foreach (FamilySymbol f in fsSet)
  {
   if (null != f)
   {
    fs = f;
    break;
   }
  }

  try
  {
   // Create a sheet view
   ViewSheet viewSheet = ViewSheet.Create(doc, fs);
   if (null == viewSheet)
    return;

   // Add current view onto the center of the sheet
   UV location = new UV(
     (viewSheet.Outline.Max.U - viewSheet.Outline.Min.U) / 2,
     (viewSheet.Outline.Max.V - viewSheet.Outline.Min.V) / 2);

   XYZ point = new XYZ(UV.U, UV.V, 0);
   Viewport.Create(doc, viewSheep.Id, doc.ActiveView.Id, point);

   // Print the sheet out
   if (viewSheet.CanBePrinted)
   {
    if (MessageBox.Show("Print the sheet?", "Revit",
MessageBoxButtons.YesNo) == DialogResult.Yes)
       viewSheet.Print();
   }
  }
  catch (Exception e)
  {
   MessageBox.Show(e.ToString());
  }
}

//============代码片段9-1：DocumentOpened事件============
public class Application_DocumentOpened : IExternalApplication
{
  public IExternalApplication.Result OnStartup(ControlledApplication application)
  {
   try
   {
    //注册事件
    application.DocumentOpened += new EventHandler
       <Autodesk.Revit.Events.DocumentOpenedEventArgs>(application_DocumentOpened);
   }
   catch (Exception)
   {
    return Autodesk.Revit.UI.Result.Failed;
   }

   return Autodesk.Revit.UI.Result.Succeeded;
  }

  public IExternalApplication.Result OnShutdown(ControlledApplication application)
  {
   //注销事件
   application.DocumentOpened -= new EventHandler
    <Autodesk.Revit.Events.DocumentOpenedEventArgs>(application_DocumentOpened);
   return Autodesk.Revit.UI.Result.Succeeded;
  }


  public void application_DocumentOpened(object sender, DocumentOpenedEventArgs args)
  {
   //从事件参数中获得文档对象
   Document doc = args.Document;

   Transaction transaction = new Transaction(doc, "Edit Address");
   if (transaction.Start() == TransactionStatus.Started)
   {
    doc.ProjectInformation.Address =
       "United States - Massachusetts - Waltham - 1560 Trapelo Road";
    transaction.Commit();
   }
  }
}

//============代码片段9-2：DocumentSavingAs事件响应函数============
private void CheckProjectStatusInitial(Object sender, DocumentSavingAsEventArgs args)
{
  Document doc = args.Document;
  ProjectInfo proInfo = doc.ProjectInformation;

  // 项目信息只存在于项目文档中。
  if (null != proInfo)
  {
   if (string.IsNullOrEmpty(proInfo.Status))
   {

    // 取消另存行为
    args.Cancel = true;
    MessageBox.Show("项目参数没有设置。取消保存。");
   }
  }
}

//============代码片段9-3：闲置事件响应函数============
//实现外部程序的Execute函数
//创建一个文字，并且注册一个闲置事件
TextNote textNote = null;
String oldDateTime = null;
public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
{
  UIApplication uiApp = new UIApplication(commandData.Application.Application);
  Document doc = commandData.Application.ActiveUIDocument.Document;
  using (Transaction t = new Transaction(doc, "Text Note Creation"))
  {
   t.Start();
   textNote = doc.Create.NewTextNote(doc.ActiveView, XYZ.Zero, XYZ.BasisX, XYZ.BasisY, 0, TextAlignFlags.TEF_ALIGN_LEFT, DateTime.Now.ToString());
   t.Commit();
  }
  oldDateTime = DateTime.Now.ToString();
  uiApp.Idling += new EventHandler<IdlingEventArgs>(idleUpdate);
  return Result.Succeeded;
}

//闲置事件处理函数
//当时间有更新就将文字内容改为当前时间
public void idleUpdate(object sender, IdlingEventArgs e)
{
  UIApplication uiApp = sender as UIApplication;
  Document doc = uiApp.ActiveUIDocument.Document;
  if (oldDateTime != DateTime.Now.ToString())
  {
   using (Transaction transaction = new Transaction(doc, "Text Note Update"))
   {
    transaction.Start();
    textNote.Text = DateTime.Now.ToString();
    transaction.Commit();
   }
   oldDateTime = DateTime.Now.ToString();
  }
}

//============代码片段9-4：实现IExternalEventHandler接口============
public class ExternalEventExample : IExternalEventHandler
{
  public void Execute(UIApplication app)
  {
   TaskDialog.Show("External Event", "Click Close to close.");
  }
  public string GetName()
  {
   return "External Event Example";
  }
}

//============代码片段9-5：外部事件的注册和触发============
public class ExternalEventExampleApp : IExternalApplication
{
  public static ExternalEventExampleApp thisApp = null;
  // 非模态对话框实例
  private ExternalEventExampleDialog m_MyForm;

  public Result OnShutdown(UIControlledApplication application)
  {
   if (m_MyForm != null && m_MyForm.Visible)
   {
    m_MyForm.Close();
   }

   return Result.Succeeded;
  }

  public Result OnStartup(UIControlledApplication application)
  {
   m_MyForm = null;   // 在外部命令中创建非模态对话框
   thisApp = this;  // 静态变量，保存Application实例

   return Result.Succeeded;
  }

  // 外部命令调用此方法
  public void ShowForm(UIApplication uiapp)
  {
   // 如果没有创建对话框，创建并显示它
   if (m_MyForm == null || m_MyForm.IsDisposed)
   {
    // 新建一个外部事件响应实例
    ExternalEventExample handler = new ExternalEventExample();

    // 新建一个外部事件实例
    ExternalEvent exEvent = ExternalEvent.Create(handler);

    // 把上面两个实例传给对话框.
    m_MyForm = new ExternalEventExampleDialog(exEvent, handler);
    m_MyForm.Show();
   }
  }
}

[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
[Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
public class Command : IExternalCommand
{
  public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
  {
   try
   {
    ExternalEventExampleApp.thisApp.ShowForm(commandData.Application);
    return Result.Succeeded;
   }
   catch (Exception ex)
   {
    message = ex.Message;
    return Result.Failed;
   }
  }
}

public partial class ExternalEventExampleDialog : Form
{
  private ExternalEvent m_ExEvent;
  private ExternalEventExample m_Handler;

  public ExternalEventExampleDialog(ExternalEvent exEvent, ExternalEventExample handler)
  {
   InitializeComponent();
   m_ExEvent = exEvent;
   m_Handler = handler;
  }

  protected override void OnFormClosed(FormClosedEventArgs e)
  {
   // 保存的实例需要释放
   m_ExEvent.Dispose();
   m_ExEvent = null;
   m_Handler = null;

   base.OnFormClosed(e);
  }

  private void closeButton_Click(object sender, EventArgs e)
  {
   Close();
  }
  //按钮响应函数，点击按钮触发外部事件
  private void showMessageButton_Click(object sender, EventArgs e)
  {
   m_ExEvent.Raise();
  }
}

//============代码片段10-1：创建一个文本框============
// 继承IExternalApplication接口
public class Ribbon : Autodesk.Revit.DB.IExternalDBApplication
{
   /// 实现OnStartup
   public Autodesk.Revit.UI.Result OnStartup(UIControlledApplication application)
   {
      // 在OnStartup函数里创建一个Ribbon文本框
      application.CreateRibbonTab("CustomTag");
      RibbonPanel panel = application.CreateRibbonPanel("CustomTag", "CustomPanel");
      TextBox textBox = panel.AddItem(new TextBoxData("CustomTextBox")) as TextBox;

      return Autodesk.Revit.UI.Result.Succeeded;
   }
   // 实现OnStartup
   public Autodesk.Revit.UI.Result OnShutdown(UIControlledApplication application)
   {
      return Autodesk.Revit.UI.Result.Succeeded;
   }
}

//============代码片段10-2：创建一个命令按钮============
PushButtonData pushButtonData = new PushButtonData("name", "Text", ECAssemblyPath, ECFullName);
PushButton pushButton = panel.AddItem(pushButtonData) as PushButton;

//============代码片段10-3：创建一个包含两个子命令按钮的下拉切换按钮============
#region 创建一个包含两个 pushButton 的 SplitButton，用来 创建结构墙和非结构墙
// 创建一个SplitButton
SplitButtonData splitButtonData = new SplitButtonData("NewWallSplit", "Create Wall");
SplitButton splitButton         = panel.AddItem(splitButtonData) as SplitButton;

// 创建一个pushButton加到SplitButton的下拉列表里
PushButton pushButton   = splitButton.AddPushButton(new PushButtonData("WallPush", "Wall",
                          AddInFullPath, "Revit.SDK.Samples.Ribbon.CS.CreateWall"));
pushButton.LargeImage   = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "CreateWall.png"), UriKind.Absolute));
pushButton.Image        = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "CreateWall-S.png"), UriKind.Absolute));
pushButton.ToolTip      = "Creates a partition wall in the building model."；
pushButton.ToolTipImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "CreateWallTooltip.bmp"), UriKind.Absolute));

// 创建另一个pushButton加到SplitButton的下拉列表里
pushButton              = splitButton.AddPushButton(new PushButtonData("StrWallPush", "Structure Wall",
                          AddInPath, "Revit.SDK.Samples.Ribbon.CS.CreateStructureWall"));
pushButton.LargeImage   = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "StrcturalWall.png"), UriKind.Absolute));
pushButton.Image        = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "StrcturalWall-S.png"), UriKind.Absolute));
#endregion

//============代码片段10-4：创建一个下拉组合框来选择墙的类型============
// Prepare data to create ComboBox
ComboBoxData comboBoxData = new ComboBoxData("WallShapeComboBox");
// Create ComboBox
ComboBox comboboxWallShape = panel.AddItem(comboBoxData) as ComboBox;

// Add options to WallShapeComboBox
// Prepare data to create ComboBoxMember
ComboBoxMemberData boxMemberData = new ComboBoxMemberData("RectangleWall","RectangleWall");
// Create ComboBoxMember
ComboBoxMember boxMember = comboboxWallShape.AddItem(boxMemberData);
boxMember.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "RectangleWall.png"), UriKind.Absolute));

// Prepare data to create ComboBoxMember
boxMemberData = new ComboBoxMemberData("CircleWall", "CircleWall");
// Create ComboBoxMember
boxMember = comboboxWallShape.AddItem(boxMemberData);
boxMember.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "CircleWall.png"), UriKind.Absolute));

// Prepare data to create ComboBoxMember
boxMemberData = new ComboBoxMemberData("TriangleWall", "TriangleWall");
// Create ComboBoxMember
boxMember = comboboxWallShape.AddItem(boxMemberData);
boxMember.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "TriangleWall.png"), UriKind.Absolute));

// Prepare data to create ComboBoxMember
boxMemberData = new ComboBoxMemberData("SquareWall", "SquareWall");
// Create ComboBoxMember
boxMember = comboboxWallShape.AddItem(boxMemberData);
boxMember.Image = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder, "SquareWall.png"), UriKind.Absolute));

//============代码片段10-5：创建一个包含两个切换按钮的切换按钮组，其中每一个切换按钮都不执行具体的ExternalCommand，仅作为选择控件使用  ============
// 创建一个RadioButtonGroup
RadioButtonGroupData radioButtonGroupData = new RadioButtonGroupData("WallTypeSelector");
RadioButtonGroup radioButtonGroup         = (RadioButtonGroup)(
                                            panel.AddItem(radioButtonGroupData));

// 给RadioButtonGroup添加toggleButton
ToggleButton toggleButton = radioButtonGroup.AddItem(
                            new ToggleButtonData("Generic8", "Generic - 8\""));
toggleButton.LargeImage   = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                            "Generic8.png"), UriKind.Absolute));
toggleButton.Image        = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                            "Generic8-S.png"), UriKind.Absolute));

// 给RadioButtonGroup添加toggleButton
toggleButton            = radioButtonGroup.AddItem(
                          new ToggleButtonData("ExteriorBrick", "Exterior - Brick"));
toggleButton.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                          "ExteriorBrick.png"), UriKind.Absolute));
toggleButton.Image      = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                          "ExteriorBrick-S.png"), UriKind.Absolute));

//============代码片段10-6：创建一个包含两个切换按钮的切换按钮组，其中每一个切换按钮都会执行具体的ExternalCommand============
// 创建一个RadioButtonGroup
RadioButtonGroupData radioButtonGroupData = new RadioButtonGroupData("WallTypeSelector");
RadioButtonGroup radioButtonGroup         = (RadioButtonGroup)(
                                            panel.AddItem(radioButtonGroupData));

// 给RadioButtonGroup添加toggleButton
ToggleButton toggleButton = radioButtonGroup.AddItem(new ToggleButtonData("Generic8",
                            "Generic - 8\"",
                            AddInPath, "Revit.SDK.Samples.Ribbon.CS.CreateGeneric8Wall"));
toggleButton.LargeImage   = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                            "Generic8.png"), UriKind.Absolute));
toggleButton.Image        = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                            "Generic8-S.png"), UriKind.Absolute));

// 给RadioButtonGroup添加toggleButton
toggleButton            = radioButtonGroup.AddItem(new ToggleButtonData("ExteriorBrick",
                          "Exterior - Brick",
                          AddInPath, "Revit.SDK.Samples.Ribbon.CS.CreateExteriorBrickWall "));
toggleButton.LargeImage = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                          "ExteriorBrick.png"), UriKind.Absolute));
toggleButton.Image      = new BitmapImage(new Uri(Path.Combine(ButtonIconsFolder,
                          "ExteriorBrick-S.png"), UriKind.Absolute));

//============代码片段10-7：EnterPresseds事件 handler============
void SetTextValue(object sender, TextBoxEnterPressedEventArgs args)
{
   string strText = m_textBox.Value as string;
}

//============代码片段10-8：外部命令中Excute函数的定义============
public Autodesk.Revit.UI.Result Execute(ExternalCommandData commandData,
            ref string message, Autodesk.Revit.DB.ElementSet elements)
{
    Application app = commandData.Application.Application;
    Document activeDoc = commandData.Application.ActiveUIDocument.Document;

    // 通过构造函数创建一个Revit任务对话框
    TaskDialog mainDialog = new TaskDialog("自定义标题");
    mainDialog.MainInstruction = "任务对话框使用说明：";
    mainDialog.MainContent =
        "这个例子演示如何通过Revit API自定义Revit样式任务对话框。";
    mainDialog.ExpandedContent = "在Revit中，任务对话框可以用于显示信息和接受简单输入。";

    // 为任务对话框添加命令链接
    mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,
                              "查看Revit Application信息");
    mainDialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink2,
                              "查看当前文档信息");

    // 设置普通按钮以及默认按钮
    mainDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
    mainDialog.DefaultButton = TaskDialogResult.Ok;

    mainDialog.VerificationText = "不再提示该消息";
    // 设置文字信息，一般为一个链接
    mainDialog.FooterText =
        "<a href=\"http://www.autodesk.com\">"
        + "点击此处了解更多</a>";

    // 显示任务对话框，并取得返回值。
    TaskDialogResult tResult = mainDialog.Show();

    // 使用对话框返回值来做进一步的事情
    if (TaskDialogResult.CommandLink1 == tResult)
    {
        TaskDialog dialog_CommandLink1 = new TaskDialog("版本信息");
        dialog_CommandLink1.MainInstruction =
            "版本名: " + app.VersionName + "\n"
            + "版本号: " + app.VersionNumber;

        dialog_CommandLink1.Show();

    }

    else if (TaskDialogResult.CommandLink2 == tResult)
    {
        // 使用静态方法创建显示任务对话框
        TaskDialog.Show("活动文档信息",
            "活动文档标题: " + activeDoc.Title + "\n"
            + "活动文档名: " + activeDoc.ActiveView.Name);
    }

    return Autodesk.Revit.UI.Result.Succeeded;
}

//============代码片段11-1：通过StructuralType区分结构柱，结构梁，结构支撑和独立基础============
public void GetStructuralType(FamilyInstance familyInstance)
{
   string message = "";
   switch (familyInstance.StructuralType)
   {
      case StructuralType.Beam: // 结构梁
         message = "FamilyInstance is a beam.";
         break;
      case StructuralType.Brace: // 结构支撑
         message = "FamilyInstance is a brace.";
         break;
      case StructuralType.Column: // 结构柱
         message = "FamilyInstance is a column.";
         break;
      case StructuralType.Footing: // 独立地基
         message = "FamilyInstance is a footing.";
         break;
      default:
         message = "FamilyInstance is non-structural or unknown framing.";
         break;
   }
}

//============代码片段11-2：获取边界条件的类别与几何信息============
//BoundaryConditions BC
// 通过BOUNDARY_CONDITIONS_TYPE参数获取此边界条件的类别
Parameter param = BC.get_Parameter(BuiltInParameter.BOUNDARY_CONDITIONS_TYPE);
switch (param.AsInteger())
{
   case 0: // 点边界条件，几何信息表现为一个点
      XYZ point = BC.Point;
      break;
   case 1: // 线边界条件，几何信息表现为一条线
      Curve curve = BC.get_Curve(0);
      break;
   case 2: // 面边界条件，几何信息表现多条线的集合
      CurveArray profile = new CurveArray();
      for (int i = 0; i < BC.NumCurves; i++)
      {
         profile.Append(BC.get_Curve(i));
      }
      break;
   default:
      break;
}

//============代码片段11-3：在结构柱上创建线边界条件============
//FamilyInstance beam
//Autodesk.Revit.Creation.Document creDocument
AnalyticalModel beamAM = beam.GetAnalyticalModel();
Curve curve = beamAM.GetCurve();
AnalyticalModelSelector selector = new AnalyticalModelSelector(curve);
selector.CurveSelector = AnalyticalCurveSelector.WholeCurve;
Reference wholeCurveRef = beamAM.GetReference(selector);

// 通过 NewLineBoundaryConditions 重载方法一创建线边界条件
BoundaryConditions createdBC1 = creDocument.NewLineBoundaryConditions(beamAM,
                                            TranslationRotationValue.Release, 0,
                                            TranslationRotationValue.Fixed, 0,
                                            TranslationRotationValue.Fixed, 0,
                                            TranslationRotationValue.Fixed, 0);
// 通过 NewLineBoundaryConditions 重载方法二创建线边界条件
BoundaryConditions createdBC2 = creDocument.NewLineBoundaryConditions(wholeCurveRef,
                                            TranslationRotationValue.Release, 0,
                                            TranslationRotationValue.Fixed, 0,
                                            TranslationRotationValue.Fixed, 0,
                                            TranslationRotationValue.Fixed, 0);

//============代码片段11-4：获取分析模型============
CurveArray profile = new CurveArray();
// API 创建一个结构楼板
Floor struFloor = RevitDoc.Create.NewFloor(profile, true);
// Regenerate revit document
RevitDoc.Regenerate();
// 获取改楼板的分析模型
AnalyticalModel floorAM = struFloor.GetAnalyticalModel();

//============代码片段11-5：获取独立基础的分析模型几何信息============
// 该元素是独立基地
FamilyInstance familyInst = element as FamilyInstance;
if (null != familyInst && familyInst.StructuralType == StructuralType.Footing)
{
   AnalyticalModel model = familyInst.GetAnalyticalModel();
   // 独立地基的分析模型被表达为一个点
   if (model.IsSinglePoint() == true)
   {
      XYZ analyticalLocationPoint = model.GetPoint();
   }
}

//============代码片段11-6：获取结构柱的分析模型几何信息============
// 该元素为结构柱
FamilyInstance familyInst = element as FamilyInstance;
if (null != familyInst && familyInst.StructuralType == StructuralType.Column)
{
   AnalyticalModel model = familyInst.GetAnalyticalModel();
   // 结构柱的分析结构被表达为一条线
   if (model.IsSingleCurve() == true)
   {
      XYZ analyticalLocationPoint = model.GetCurve ();
   }
}

//============代码片段11-7：获取结构墙的分析模型几何信息============
Wall wall = null;
// 获取结构墙的分析模型
AnalyticalModel amWall = wall.GetAnalyticalModel() as AnalyticalModel;
if (null == amWall)
{
   // 建筑墙没有分析模型
   return;
}
// 获取分析模型曲线
int modelCurveNum = amWall.GetCurves(AnalyticalCurveType.ActiveCurves).Count;

//============代码片段11-8：获取独立基础的分析模型点的参照============
// element: 一个独立基础
// 获取独立基础的分析模型
AnalyticalModel am = element.GetAnalyticalModel();
AnalyticalModelSelector amSelector = new AnalyticalModelSelector();
// 获取独立基础的分析模型点的几何引用
Reference refer = am.GetReference(amSelector);

//============代码片段11-9：获取结构梁的分析模型曲线的几何引用============
//element: 一个结构梁
// 获取结构梁的分析模型
AnalyticalModel am = element.GetAnalyticalModel();

// 获取结构梁的分析模型曲线的几何引用
AnalyticalModelSelector amSelector = new AnalyticalModelSelector(AnalyticalCurveSelector.WholeCurve);
Reference wholeCurveRefer = am.GetReference(amSelector);

// 获取结构梁的分析模型曲线的起点的几何引用
amSelector = new AnalyticalModelSelector(AnalyticalCurveSelector.StartPoint);
Reference curveStartPointRefer = am.GetReference(amSelector);

// 获取结构梁的分析模型曲线的终点的几何引用
amSelector = new AnalyticalModelSelector(AnalyticalCurveSelector.EndPoint);
Reference curveEndPointRefer = am.GetReference(amSelector);

//============代码片段11-10：获取结构墙的分析模型曲线的参照============
//element: 一个结构墙
// 获取结构墙的分析模型
AnalyticalModel am = element.GetAnalyticalModel();
// 获取结构墙分析模型的曲线列表
IList<Curve> amCurveList = am.GetCurves(AnalyticalCurveType.ActiveCurves);

for (int curveIndex = 0; curveIndex < amCurveList.Count; curveIndex++)
{
   // 获取结构墙的指定的分析模型曲线的几何引用
   AnalyticalModelSelector amSelector = new AnalyticalModelSelector(amCurveList[curveIndex], AnalyticalCurveSelector.WholeCurve);
   Reference wholeCurveRefer = am.GetReference(amSelector);

   // 获取结构墙的指定的分析模型曲线的起点的几何引用
   amSelector = new AnalyticalModelSelector(amCurveList[curveIndex], AnalyticalCurveSelector.StartPoint);
   Reference curveStartPointRefer = am.GetReference(amSelector);

   // 获取结构墙的指定的分析模型曲线的终点的几何引用
   amSelector = new AnalyticalModelSelector(amCurveList[curveIndex], AnalyticalCurveSelector.EndPoint);
   Reference curveEndPointRefer = am.GetReference(amSelector);
}

//============代码片段11-11：设置结构柱分析模型底部Z方向投影为 柱底部============
am.SetAlignmentMethod(AnalyticalElementSelector.StartOrBase, AnalyticalDirection.Z, AnalyticalAlignmentMethod.Projection);
am.SetAnalyticalProjectionType(AnalyticalElementSelector.StartOrBase, AnalyticalDirection.Z, AnalyticalProjectionType.Bottom);

//============代码片段11-12：设置结构柱分析模型底部Z方向投影为 标高 1============
am.SetAlignmentMethod(AnalyticalElementSelector.StartOrBase, AnalyticalDirection.Z, AnalyticalAlignmentMethod.Projection);
am.SetAnalyticalProjectionDatumPlane(AnalyticalElementSelector.StartOrBase, AnalyticalDirection.Z, level1Id);

//============代码片段11-13：设置结构梁分析模型的近似曲线============
if (am.CanApproximate())
{
   am.Approximate(true);
   am.SetApproximationDeviation(1.5);
   am.SetUsesHardPoints(false);
}

//============代码片段11-14：设置分析模型的 分析为============
if(am.IsAnalyzeAsValid(AnalyzeAs.Gravity))
{
   am.SetAnalyzeAs(AnalyzeAs.Gravity);
}

//============代码片段11-15：楼板的支撑信息============
public void GetSupportInfo_Floor(Floor floor)
{
   // floor : 楼板
   // 获取楼板的分析模型
   AnalyticalModel amFloor = floor.GetAnalyticalModel();
   // 在这里IsElementFullySupported方法将返回true，这表明楼板被完全支撑
   bool fullySuported = amFloor.IsElementFullySupported();

   // 获取具体的支撑信息，将返回含有三个support成员的列表。每一个support都对应一根结构梁对地板提供的支撑
   IList<AnalyticalModelSupport> supportList = amFloor.GetAnalyticalModelSupports();
   foreach (AnalyticalModelSupport support in supportList)
   {
      // 返回当前提供支撑的分析梁的id
      ElementId id = support.GetSupportingElement();
      // 返回CurveSupport，表明该支撑是一个线性支撑
      AnalyticalSupportType supportType = support.GetSupportType();

      // 根据支撑力类型的不同，可以分别通过GetPoint，GetCurve，GetFace这三种方法之一获取支撑力的具体几何位置
      switch (supportType)
      {
         case AnalyticalSupportType.PointSupport:
            XYZ point = support.GetPoint();
            break;
            case AnalyticalSupportType.CurveSupport:
            Curve curve = support.GetCurve();
            break;
         case AnalyticalSupportType.SurfaceSupport:
            Face face = support.GetFace();
            break;
         case AnalyticalSupportType.UnknownSupport:
            break;
      }

      // 通过GetPriority方法可以获取当前支撑力的优先权。当一个物体被多项支撑时，该属性可对提供的支撑进行排序，从而可选取最优的支撑
      AnalyticalSupportPriority supportPriority = support.GetPriority();
   }
}

//============代码片段11-16：楼板的支撑信息============
// 楼板的支撑信息
public void GetSupportInfo_Floor(Floor floor)
{
   // floor : 楼板
   // 获取楼板的分析模型
   AnalyticalModel amFloor = floor.GetAnalyticalModel();
   // 在这里IsElementFullySupported方法将返回false，这表明楼板未被完全支撑
   bool fullySuported = amFloor.IsElementFullySupported();

   // 获取具体的支撑信息，将返回一个空列表。表示该楼板没有任何支撑物
   IList<AnalyticalModelSupport> supportList = amFloor.GetAnalyticalModelSupports();
   If(supportList.Count == 0) string message = “楼板没有支撑”;
}

//============代码片段11-17：墙的支撑信息============
// 该实例中墙的支撑信息
public void GetSupportInfo_Walls(Wall wall)
{
   // wall : 四面墙中的任何一面墙
   // 获取墙的分析模型
   AnalyticalModel amWall = wall.GetAnalyticalModel();
   // 在这里IsElementFullySupported方法将返回true，这表明墙被完全支撑
   bool fullySuported = amWall.IsElementFullySupported();

   // 获取具体的支撑信息，这里返回只含有一个support成员的列表。这个support对应墙下面的楼板
   IList<AnalyticalModelSupport> supportList = amWall.GetAnalyticalModelSupports();
   foreach (AnalyticalModelSupport support in supportList)
   {
      // 返回当前提供支撑的楼板的分析模型的id
      ElementId id = support.GetSupportingElement();
      // 返回CurveSupport，表明该支撑是一个线性支撑
      AnalyticalSupportType supportType = support.GetSupportType();

      // 根据支撑力类型的不同，可以分别通过GetPoint，GetCurve，GetFace这三种方法之一获取支撑力的具体几何位置
      switch (supportType)
      {
         case AnalyticalSupportType.PointSupport:
            XYZ point = support.GetPoint();
            break;
            case AnalyticalSupportType.CurveSupport:
            Curve curve = support.GetCurve();
            break;
         case AnalyticalSupportType.SurfaceSupport:
            Face face = support.GetFace();
            break;
         case AnalyticalSupportType.UnknownSupport:
            break;
      }

      AnalyticalSupportPriority supportPriority = support.GetPriority();
   }
}

//============代码片段11-18：结构梁的支撑信息============
// 该实例中梁的支撑信息
public void GetSupportInfo_Beam(FamilyInstance beam)
{
   // beam: 结构梁
   // 获取梁的分析模型
   AnalyticalModel amBeam = beam.GetAnalyticalModel();
   // 在这里IsElementFullySupported方法将返回false，这表明梁未被完全支撑
   bool fullySuported = amBeam.IsElementFullySupported();

   // 获取具体的支撑信息，这里返回只含有一个support成员的列表。这个support对应该示例中的结构柱
   IList<AnalyticalModelSupport> supportList = amBeam.GetAnalyticalModelSupports();
   foreach (AnalyticalModelSupport support in supportList)
   {
      // 返回提供该支撑的结构柱的分析模型的id
      ElementId id = support.GetSupportingElement();
      // 返回PointSupport，表明该支撑是一个点支撑
      AnalyticalSupportType supportType = support.GetSupportType();

      // 根据支撑力类型的不同，可以分别通过GetPoint，GetCurve，GetFace这三种方法之一获取支撑力的具体几何位置
      switch (supportType)
      {
         case AnalyticalSupportType.PointSupport:
            XYZ point = support.GetPoint();
            break;
            case AnalyticalSupportType.CurveSupport:
            Curve curve = support.GetCurve();
            break;
         case AnalyticalSupportType.SurfaceSupport:
            Face face = support.GetFace();
            break;
         case AnalyticalSupportType.UnknownSupport:
            break;
      }

      AnalyticalSupportPriority supportPriority = support.GetPriority();
   }
}

//============代码片段11-19：调整结构柱分析模型的顶端向上（Z方向）偏移5============
// columnAnalytical 结构柱的分析模型
AnalyticalElementSelector elemSelector = AnalyticalElementSelector.EndOrTop;
XYZ offset = new XYZ(0, 0, 5);
columnAnalytical.SetOffset(elemSelector, offset);

// 返回 XYZ(0, 0, 5);
XYZ regetOffset = columnAnalytical.GetOffset(elemSelector);

//============代码片段11-20：通过手动调整使结构柱的分析模型与某结构梁的分析模型相连接============
// columnAnalytical:分析柱
// beamAnalytical 分析梁
// 判断该分析模型支持手动调整
if(columnAnalytical.SupportsManualAdjustment())
{
   Reference sourceRef = columnAnalytical.GetReference(new AnalyticalModelSelector(AnalyticalCurveSelector.EndPoint));
   Reference targetRef = beamAnalytical.GetReference(new AnalyticalModelSelector(AnalyticalCurveSelector.EndPoint));
   // 手动调整分析柱的终点到分析梁的终点
   columnAnalytical.ManuallyAdjust(sourceRef, targetRef);
   bool isManuallyAdjusted = columnAnalytical.IsManuallyAdjusted());//true

   // 将分析模型设置回默认位置
   columnAnalytical.ResetManualAdjustment();
   isManuallyAdjusted = columnAnalytical.IsManuallyAdjusted()); //true
}

//============代码片段11-21：读取Revit文档对象里所有的分析链接，包括自动创建的和手动创建的============
public void ReadAnalyticalLinks(Document document)
{
   // 过滤出所有的分析链接元素
   FilteredElementCollector collectorAnalyticalLinks = new FilteredElementCollector(document);
   collectorAnalyticalLinks.OfClass(typeof(AnalyticalLink));

  IEnumerable<AnalyticalLink> alinks = collectorAnalyticalLinks.
                                       ToElements().Cast<AnalyticalLink>();
  int nAutoGeneratedLinks = 0;
  int nManualLinks = 0;
  foreach (AnalyticalLink alink in alinks)
  {
      // 统计Revit自动创建的分析链接
      if (alink.IsAutoGenerated() == true)
         nAutoGeneratedLinks++;
      else // 统计手动创建的分析链接
         nManualLinks++;
   }
   string msg = "Auto-generated AnalyticalLinks: " + nAutoGeneratedLinks;
   msg += "\nManually created AnalyticalLinks: " + nManualLinks;
   TaskDialog.Show("AnalyticalLinks", msg);
}

//============代码片段11-22：选择两个族实例创建分析链接============
public void CreateLink(Document doc, FamilyInstance fi1, FamilyInstance fi2)
{
   // 得到该Document里所有的分析节点
   FilteredElementCollector hubCollector = new FilteredElementCollector(doc);
   hubCollector.OfClass(typeof(Hub));
   ICollection<Element> allHubs = hubCollector.ToElements();
   // 得到第一个AnalyticalLinkType
   FilteredElementCollector linktypeCollector = new FilteredElementCollector(doc);
   linktypeCollector.OfClass(typeof(AnalyticalLinkType));
   ElementId firstLinkType = linktypeCollector.ToElementIds().First();

   // 得到指定的族实例的分析节点
   ElementId startHubId = GetHub(fi1.GetAnalyticalModel().Id, allHubs);
   ElementId endHubId = GetHub(fi2.GetAnalyticalModel().Id, allHubs);

   Transaction tran = new Transaction(doc, "Create Link");
   tran.Start();
   // 通过指定两个分析节点来创建分析链接
   AnalyticalLink createdLink = AnalyticalLink.Create(doc, firstLinkType, startHubId, endHubId);
   tran.Commit();
}

/// <summary>
/// 获取指定的分析模型的第一个分析节点
/// </summary>
/// <param name="hostId">分析模型</param>
/// <param name="allHubs">分析节点集合</param>
/// <returns>返回指定的分析模型的第一个分析节点</returns>
private ElementId GetHub(ElementId hostId, ICollection<Element> allHubs)
{
   foreach (Element ehub in allHubs)
   {
      Hub hub = ehub as Hub;
      ConnectorManager manager = hub.GetHubConnectorManager();
      ConnectorSet connectors = manager.Connectors;
      foreach (Connector connector in connectors)
      {
         ConnectorSet refConnectors = connector.AllRefs;
         foreach (Connector refConnector in refConnectors)
         {
            if (refConnector.Owner.Id == hostId)
            {
               return hub.Id;
            }
         }
      }
   }
   return ElementId.InvalidElementId;
}

//============代码片段12-1 读写外观元素============
public void GetAndSetMaterialAppearance(Document doc, Material mat, Asset anotherAsset)
{
    ElementId assetElementId = mat.AppearanceAssetId;
    if (assetElementId != ElementId.InvalidElementId)
    {
        // 获取材料的外观元素(AppearanceAssetElement)实例
        AppearanceAssetElement appearanceElement =
            doc.GetElement(assetElementId) as AppearanceAssetElement;

        // 读写外观的属性集合（Asset）
        Asset currentAsset = appearanceElement.GetRenderingAsset();
        appearanceElement.SetRenderingAsset(anotherAsset);
    }

    // 设置材料的外观元素。
    FilteredElementCollector cotr = new FilteredElementCollector(doc);
    IEnumerable<AppearanceAssetElement> allAppearanceElements =
        cotr.OfClass(typeof(AppearanceAssetElement)).Cast<AppearanceAssetElement>();
    mat.AppearanceAssetId = allAppearanceElements.First<AppearanceAssetElement>().Id;
}

//============代码片段12-2 加载外观元素到文档============
public void LoadAndGetAllAppearanceAssets(
    Autodesk.Revit.ApplicationServices.Application revitApp)
{
    AssetSet theAssetSet = revitApp.get_Assets(AssetType.Appearance);
    foreach (Asset theAsset in theAssetSet)
    {
        // 你可以在这里读取用ＡＰＩ加载的外观元素了。
    }
}

//============代码片段12-3 创建材料的物理和热度============
/// <summary>
/// Create a new brick material
/// </summary>
/// <returns>The specific material</returns>
private Material CreateSampleBrickMaterial()
{
    SubTransaction createMaterial = new SubTransaction(this.m_document.Document);
    createMaterial.Start();
    Material materialNew = null;

    //Try to copy an existing material.  If it is not available, create a new one.
    Material masonry_Brick = GetMaterial("Brick, Common");
    if (masonry_Brick != null)
    {
        materialNew = masonry_Brick.Duplicate(masonry_Brick.Name + "_new");
        System.Diagnostics.Debug.WriteLine(masonry_Brick.MaterialClass);
        materialNew.MaterialClass = "Brick";
    }
    else
    {
        ElementId idNew = Material.Create(m_document.Document, "New Brick Sample");
        materialNew = m_document.Document.GetElement(idNew) as Material;
        materialNew.Color = new Autodesk.Revit.DB.Color(255, 0, 0);
    }
    createMaterial.Commit();

    SubTransaction createPropertySets = new SubTransaction(this.m_document.Document);
    createPropertySets.Start();

    //Create a new structural asset and set properties on it.
    StructuralAsset structuralAsssetBrick = new StructuralAsset("BrickStructuralAsset" , Autodesk.Revit.DB.StructuralAssetClass.Generic);
    structuralAsssetBrick.DampingRatio = .5;

    PropertySetElement pseStructural = PropertySetElement.Create(m_document.Document, structuralAsssetBrick);


    //Create a new thermal asset and set properties on it.
    ThermalAsset thermalAssetBrick = new ThermalAsset("BrickThermalAsset", Autodesk.Revit.DB.ThermalMaterialType.Solid);
    thermalAssetBrick.Porosity = 0.1;
    thermalAssetBrick.Permeability = 0.2;
    thermalAssetBrick.Compressibility = .5;
    thermalAssetBrick.ThermalConductivity = .5;

    //Create PropertySets from assets and assign them to the material.
    PropertySetElement pseThermal = PropertySetElement.Create(m_document.Document, thermalAssetBrick);
    createPropertySets.Commit();
    SubTransaction setPropertySets = new SubTransaction(this.m_document.Document);
    setPropertySets.Start();
    materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pseStructural.Id);
    materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Thermal, pseThermal.Id);

    //also try
    //materialNew.ThermalAssetId = pseThermal.Id;

    setPropertySets.Commit();
    return materialNew;
}

//============代码片段12-4为墙体设置材料============
public void GetAndSetMaterialForWall(Document doc, Wall wall, Material newMat)
{
    // 得到墙体的复合结构（CompoundStructure）实例
    WallType wallType = wall.WallType;
    CompoundStructure wallCS = wallType.GetCompoundStructure();

    // 得到墙体第一层材料的元素Id
    CompoundStructureLayer firstLayer = wallCS.GetLayers().First<CompoundStructureLayer>();
    ElementId currentMatId = firstLayer.MaterialId;

    // 为墙体第一层设置一个新材料
    firstLayer.MaterialId = newMat.Id;
}

//============代码片段13-1：创建风管============
public static Duct CreateDuct(Document doc)
{
    ElementId systemTypeId, ductTypeId, levelId;
    systemTypeId = ductTypeId = levelId = ElementId.InvalidElementId;

    // 获取标高Id
    var levelFilter = new ElementClassFilter(typeof(Level));
    FilteredElementCollector levels = new FilteredElementCollector(doc);
    levels = levels.WherePasses(levelFilter);
    foreach (Level level in levels)
    {
        if (level.Name == "Level 1")
        {
            levelId = level.Id;
            break;
        }
    }
    if(levelId == ElementId.InvalidElementId)
        throw new Exception("无法标高");

    // 获取类型为SupplyAir的系统类型
    var systemTypeFilter = new ElementClassFilter(typeof(MEPSystemType));
    FilteredElementCollector systemTypes = new FilteredElementCollector(doc);
    systemTypes = systemTypes.WherePasses(systemTypeFilter);
    List<MEPSystemType> systypes = new List<MEPSystemType>();
    foreach (MEPSystemType element in systemTypes)
    {
        if (element.SystemClassification == MEPSystemClassification.SupplyAir)
        {
            systemTypeId = element.Id;
            break;
        }
    }
    if (systemTypeId == ElementId.InvalidElementId)
        throw new Exception("无法找到系统类型");

    // 获取风管类型
    var ductTypeFilter = new ElementClassFilter(typeof(DuctType));
    FilteredElementCollector ductTypes = new FilteredElementCollector(doc);
    var result = ductTypes.WherePasses(ductTypeFilter).ToList();
    foreach (DuctType element in result)
    {
        ductTypeId = element.Id;
        break;
    }

    // 创建风管
    using (Transaction transaction = new Transaction(doc))
    {
        transaction.Start("创建风管");
        Duct duct = Duct.Create(doc, systemTypeId, ductTypeId, levelId, new XYZ(0, 10, 0), new XYZ(10, 0, 0));
        transaction.Commit();
        return duct;
    }
}

//============代码片段13-2：============
public void GetElementAtConnector(Connector connector)
{
   MEPSystem mepSystem = connector.MEPSystem;
   if (null != mepSystem)
   {
      string message = "Connector is owned by: " + connector.Owner.Name;
      if (connector.IsConnected == true)
      {
         ConnectorSet connectorSet = connector.AllRefs;
         ConnectorSetIterator csi = connectorSet.ForwardIterator();
         while (csi.MoveNext())
         {
            Connector connected = csi.Current as Connector;
            if (null != connected)
            {
               // look for physical connections
               if (connected.ConnectorType == ConnectorType.EndConn ||
                  connected.ConnectorType == ConnectorType.CurveConn ||
                  connected.ConnectorType == ConnectorType.PhysicalConn)
               {
                  message += "\nConnector is connected to: " + connected.Owner.Name;
                  message += "\nConnection type is: " + connected.ConnectorType;
               }
            }
         }
      }
      else
      {
         message += "\nConnector is not connected to anything.";
      }
      MessageBox.Show(message, "Revit");
   }
}

//============代码片段13-3：创建弯头============
public static void ConnectTwoDuctsWithElbow(Document doc, Duct duct1, Duct duct2)
{
    double minDistance = double.MaxValue;
    Connector connector1, connector2;
    connector1 = connector2 = null;

    // 找到距离最近的两个Connector
    foreach (Connector con1 in duct1.ConnectorManager.Connectors)
    {
        foreach (Connector con2 in duct2.ConnectorManager.Connectors)
        {
            var dis = con1.Origin.DistanceTo(con2.Origin);
            if (dis < minDistance)
            {
                minDistance = dis;
                connector1 = con1;
                connector2 = con2;
            }
        }
    }
    if (connector1 != null && connector2 != null)
    {
        using (Transaction transaction = new Transaction(doc))
        {
            transaction.Start("Create Elbow");
            doc.Create.NewElbowFitting(connector1, connector2);
            transaction.Commit();
        }
    }
}

//============代码片段13-4：============
public static void CreateMechanicalSystem(Document doc, DuctSystemType systemType, FamilyInstance equipment, params FamilyInstance[] airTerminals)
{
    var me = equipment.MEPModel as MEPModel;
    if (me != null)
    {
        // 获取符合条件的设备上的电气连接件
        var connsOfEquipment = GetConnector(me.ConnectorManager, systemType).ToList();
        if (connsOfEquipment.Count > 0)
        {
            // 选取设备上的第一个电气连接件
            var equipmentConn = connsOfEquipment[0];

            var connsOfTerminals = new ConnectorSet();
            foreach (var terminal in airTerminals)
            {
                // 获取符合条件的散流器上的电气连接件，取第一个
                var airTerminalEquipment = terminal.MEPModel as MEPModel;
                var airConns = GetConnector(airTerminalEquipment.ConnectorManager, systemType);
                var airConn = airConns.FirstOrDefault();
                if (airConn != null)
                    connsOfTerminals.Insert(airConn);
            }

            // 创建风管系统
            using (Transaction transaction = new Transaction(doc))
            {
                transaction.Start("Create System");
                doc.Create.NewMechanicalSystem(equipmentConn, connsOfTerminals, systemType);
                transaction.Commit();
            }
        }
    }
}

// 获取符合管道系统的电气连接件
private static IEnumerable<Connector> GetConnector(ConnectorManager conMgr, DuctSystemType systemType)
{
    foreach (Connector conn in conMgr.Connectors)
    {
        // Domain为Hvac，否则DuctSystemType属性会抛出异常
        if (conn.Domain == Domain.DomainHvac && conn.DuctSystemType == systemType)
        {
            yield return conn;
        }
    }
}

//============代码片段13-5：============
using (Transaction tran = new Transaction(doc, "DuctSettings"))
{
   tran.Start();
   //获取Revit的风管设置对象
   DuctSettings ductSettings = DuctSettings.GetDuctSettings(doc);
   //设置风管设置的角度参数
   ductSettings.FittingAngleUsage = FittingAngleUsage.UseAnAngleIncrement;
   tran.Commit();
}

//============代码片段13-6：添加电压============
using (Transaction tran = new Transaction(doc, "ElectricalSetting"))
{
   tran.Start();
   //获取Revit的电气设置对象
   ElectricalSetting electricalSettings = ElectricalSetting.GetElectricalSettings(doc);
   //向电压定义参数中加入220伏电压
   electricalSettings.AddVoltageType("220伏电压", 220, 220, 220);
   tran.Commit();
}

//============代码片段13-7：创建分区和空间============
using (Transaction tran = new Transaction(doc, "Create new space"))
{
   tran.Start();
   Level level1 = doc.GetElement(new ElementId(311)) as Level;
   Phase phase1 = doc.GetElement(new ElementId(86961)) as Phase;
   //用时期(Phase)创建空间
   Space space1 = doc.Create.NewSpace(phase1);
   //用标高(Level)和平面的点创建空间
   Space space2 = doc.Create.NewSpace(level1, new UV(-31, -19));
   //用时期(Phase)，标高(Level)和平面的点创建空间
   Space space3 = doc.Create.NewSpace(level1, phase1, new UV(4, 16));
  //用标高(Level)和时期(Phase)创建分区
   Zone zone1 = doc.Create.NewZone(level1, phase1);
   SpaceSet spaceSet = new SpaceSet();
   spaceSet.Insert(space1);
   spaceSet.Insert(space2);
   spaceSet.Insert(space3);
   //将空间添加到分区中
   zone1.AddSpaces(spaceSet);
   tran.Commit();
}

//============代码片段14-1：创建一个文字注释元素============
public void MyFirstMacroAppCS()
{
  Autodesk.Revit.DB.XYZ baseVec = Application.Create.NewXYZ(1.0, 0.0, 0.0);
  Autodesk.Revit.DB.XYZ upVec = Application.Create.NewXYZ(0.0, 0.0, 1.0);
  Autodesk.Revit.DB.XYZ origin = Application.Create.NewXYZ(0.0, 0.0, 0.0);

  Autodesk.Revit.DB.TextAlignFlags align = Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_LEFT | Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_TOP;

  string strText = "My First Macro, App level, C#!";
  double lineWidth = 4.0 / 12.0;

  Autodesk.Revit.DB.View pView = ActiveUIDocument.Document.ActiveView;
  Autodesk.Revit.DB.Transaction t = new Autodesk.Revit.DB.Transaction(ActiveUIDocument.Document, "NewTextNote");
  t.Start();
  ActiveUIDocument.Document.Create.NewTextNote(pView, origin, baseVec, upVec, lineWidth, align, strText);

  t.Commit();
}

//============代码片段14-2：创建一个文字注释元素（C#）============
public void MyFirstMacroAppCS()
{
  Autodesk.Revit.DB.XYZ baseVec = Application.Create.NewXYZ(1.0, 0.0, 0.0);
  Autodesk.Revit.DB.XYZ upVec = Application.Create.NewXYZ(0.0, 0.0, 1.0);
  Autodesk.Revit.DB.XYZ origin = Application.Create.NewXYZ(0.0, 0.0, 0.0);

  Autodesk.Revit.DB.TextAlignFlags align = Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_LEFT | Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_TOP;

  string strText = "My First Macro, App level, C#!";
  double lineWidth = 4.0 / 12.0;

  Autodesk.Revit.DB.View pView = ActiveUIDocument.Document.ActiveView;
  Autodesk.Revit.DB.Transaction t = new Autodesk.Revit.DB.Transaction(ActiveUIDocument.Document, "NewTextNote");
  t.Start();
  ActiveUIDocument.Document.Create.NewTextNote(pView, origin, baseVec, upVec, lineWidth, align, strText);

  t.Commit();
}

//============代码片段14-3：创建一个文字注释元素（VB.NET）============
Public Sub MyFirstMacroAppVB()
  Dim baseVec As Autodesk.Revit.DB.XYZ = Application.Create.NewXYZ(1.0, 0.0, 0.0)
  Dim upVec As Autodesk.Revit.DB.XYZ = Application.Create.NewXYZ(0.0, 0.0, 1.0)
  Dim origin As Autodesk.Revit.DB.XYZ = Application.Create.NewXYZ(0.0, 0.0, 0.0)
  Dim align As Autodesk.Revit.DB.TextAlignFlags = Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_LEFT Or Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_TOP
  Dim strText As String = "My First Macro, App Level, VB.NET!"
  Dim lineWidth As Double = 4.0 / 12.0
  Dim pView As Autodesk.Revit.DB.View = ActiveUIDocument.Document.ActiveView
  Dim Transaction As Autodesk.Revit.DB.Transaction = New Autodesk.Revit.DB.Transaction(ActiveUIDocument.Document, "NewTextNote")
  Transaction.Start()
  ActiveUIDocument.Document.Create.NewTextNote(pView, origin, baseVec, upVec, lineWidth, align, strText)
  Transaction.Commit()
End Sub

//============代码片段14-4：显示版本和文件标题（Ruby）============
def SampleMacroImplementation()
  revitVersion = self.Application.VersionBuild
  docTitle = ""
  if (self.ActiveUIDocument.Document!= nil)
    docTitle = self.ActiveUIDocument.Document.Title
  end
  TaskDialog.Show("Ruby Sample", "Revit Version: " + revitVersion + ", Document title: " + docTitle)
end

//============代码片段14-5：显示版本和文件标题（Python）============
def SampleMacro(self):
  revitVersion = self.Application.VersionBuild
  docTitle = ""
  if (self.ActiveUIDocument.Document!= None):
    docTitle = self.ActiveUIDocument.Document.Title;
  TaskDialog.Show("Python Sample", "Revit Version: " + revitVersion + ", Document title: " + docTitle)

//============代码片段14-6：创建一个文字注释元素（C#）============
public void MyFirstMacroDocCS()
{
  Autodesk.Revit.DB.XYZ baseVec = Document.Application.Create.NewXYZ(0.0, 0.0, 1.0);
  Autodesk.Revit.DB.XYZ upVec = Document.Application.Create.NewXYZ(0.0, 0.0, 1.0);
  Autodesk.Revit.DB.XYZ origin = Document.Application.Create.NewXYZ(0.0, 0.0, 0.0);
  Autodesk.Revit.DB.TextAlignFlags align = Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_LEFT | Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_TOP;
  string strText = "My First Macro, Doc level, C#!";
  double lineWidth = 4.0 / 12.0;
  Autodesk.Revit.DB.Transaction t = new Autodesk.Revit.DB.Transaction(Document, "NewTextNote");
  t.Start();
  Autodesk.Revit.DB.View pView = Document.ActiveView;
  Document.Create.NewTextNote(pView, origin, baseVec, upVec, lineWidth, align, strText);
  t.Commit();
}

//============代码片段14-7：创建一个文字注释元素（VB.NET）============
Public Sub MyFirstMacroDocVB()
  Dim baseVec As Autodesk.Revit.DB.XYZ = Document.Application.Create.NewXYZ(1.0, 0.0, 0.0)
  Dim upVec As Autodesk.Revit.DB.XYZ = Document.Application.Create.NewXYZ(0.0, 0.0, 1.0)
  Dim origin As Autodesk.Revit.DB.XYZ = Document.Application.Create.NewXYZ(0.0, 0.0, 0.0)
  Dim align As Autodesk.Revit.DB.TextAlignFlags = Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_LEFT Or Autodesk.Revit.DB.TextAlignFlags.TEF_ALIGN_TOP
  Dim strText As String = "My First Macro, Doc Level, VB.NET!"
  Dim lineWidth As Double = 4.0 / 12.0
  Dim pView As Autodesk.Revit.DB.View = Document.ActiveView
  Dim Transaction As Autodesk.Revit.DB.Transaction = New Autodesk.Revit.DB.Transaction(Document, "NewTextNote")
  Transaction.Start()
  Document.Create.NewTextNote(pView, origin, baseVec, upVec, lineWidth, align, strText)
  Transaction.Commit()
End Sub

//============代码片段14-8：显示版本和文件标题（Ruby）============
def SampleMacroImplementation()
  revitVersion = self.Application.Application.VersionBuild
  docTitle = ""
  if (self.Document!= nil)
  docTitle = self.Document.Title
  end
  TaskDialog.Show("Ruby Sample", "Revit Version: " + revitVersion + ", Document title: " + docTitle)
end

//============代码片段14-9：显示版本和文件标题（Python）============
def SampleMacro(self):
  revitVersion = self.Application.Application.VersionBuild
  docTitle = ""
  if (self.Document!= None):
    docTitle = self.Document.Title;
  TaskDialog.Show("Python Sample", "Revit Version: " + revitVersion + ", Document title: " + docTitle)

//============代码片段15-1：HelloRevit（VB.NET）============
Imports System

Imports Autodesk
Imports Autodesk.Revit.DB
Imports Autodesk.Revit.UI
Imports Autodesk.Revit.ApplicationServices

<Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)> _
Public Class Command
    Implements Autodesk.Revit.UI.IExternalCommand

    Public Function Execute(ByVal commandData As Autodesk.Revit.UI.ExternalCommandData, _
        ByRef message As String, ByVal elements As Autodesk.Revit.DB.ElementSet) _
        As Autodesk.Revit.UI.Result Implements Autodesk.Revit.UI.IExternalCommand.Execute

        Dim app As Application
        app = commandData.Application.Application
        Dim activeDoc As Document
        activeDoc = commandData.Application.ActiveUIDocument.Document
        Dim mainDialog As TaskDialog
        mainDialog = New TaskDialog("Hello, Revit!")
        mainDialog.MainInstruction = "Hello, Revit!"
        mainDialog.MainContent = _
                "This sample shows how a basic ExternalCommand can be added to the Revit user interface." _
                + " It uses a Revit task dialog to communicate information to the interactive user.\n"
        mainDialog.Show()
        Return Autodesk.Revit.UI.Result.Succeeded
    End Function
End Class

//============代码片段15-2：HelloRevit.addin文件============
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Assembly>HelloRevit.dll</Assembly>
    <ClientId>0e6385db-b2c0-4397-8702-b1b889bad37c</ClientId>
    <FullClassName>HelloRevit.Class1</FullClassName>
    <VendorId>ADSK</VendorId>
  </AddIn>
</RevitAddIns>

//============代码片段15-3：HelloRevit.h============
#pragma once

using namespace System;

using namespace Autodesk::Revit;
using namespace Autodesk::Revit::DB;
using namespace Autodesk::Revit::UI;
using namespace Autodesk::Revit::ApplicationServices;

namespace HelloRevit {

  public ref class Class1 : IExternalCommand
  {
  public:
    virtual Result Execute(ExternalCommandData^ commandData, String^% message, ElementSet^ elements);
  };
}

//============代码片段15-4：HelloRevit.cpp============
#include "stdafx.h"

#include "HelloRevit.h"

namespace HelloRevit
{
  Result Class1::Execute(ExternalCommandData^ commandData, String^% message, ElementSet^ elements)
  {
    Application^ app = commandData->Application->Application;
    Document^ activeDoc = commandData->Application->ActiveUIDocument->Document;
    TaskDialog^ mainDialog = gcnew TaskDialog("Hello, Revit!");
    mainDialog->MainInstruction = "Hello, Revit!";
    mainDialog->MainContent = "This sample shows how a basic ExternalCommand can be added to the Revit user interface. It uses a Revit task dialog to communicate information to the interactive user.";
    mainDialog->Show();
    return Autodesk::Revit::UI::Result::Succeeded;
  }
}

//============代码片段15-5：HelloRevit.addin文件============
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Assembly>HelloRevit.dll</Assembly>
    <ClientId>0e6385db-b2c0-4397-8702-b1b889bad37c</ClientId>
    <FullClassName>HelloRevit.Class1</FullClassName>
    <VendorId>ADSK</VendorId>
  </AddIn>
</RevitAddIns>

//============代码片段15-6：HelloRevit.h============
module Module1

open Autodesk.Revit
open Autodesk.Revit.UI
open Autodesk.Revit.Attributes
open Autodesk.Revit.DB

[<Transaction(TransactionMode.Manual)>]
type Class1 =
    interface Autodesk.Revit.UI.IExternalCommand with
        member this.Execute(commandData,
                            message : string byref,
                            elements ) =
            let uiApp = commandData.Application
            let app  = uiApp.Application
            let uiDoc = uiApp.ActiveUIDocument
            let doc  = uiDoc.Document

            use mainDialog = new TaskDialog("Hello, Revit!")
            mainDialog.MainInstruction <- "This sample shows how a basic ExternalCommand can be added to the Revit user interface. It uses a Revit task dialog to communicate information to the interactive user."
            let result = mainDialog.Show()
            Autodesk.Revit.UI.Result.Succeeded

//============代码片段15-7：HelloRevit.addin文件============
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Assembly>HelloRevit.dll</Assembly>
    <ClientId>1585c811-3dc4-43f4-9ec5-e7e695a6ff72</ClientId>
    <FullClassName>HelloRevit.Class1</FullClassName>
    <VendorId>ADSK</VendorId>
  </AddIn>
</RevitAddIns>

