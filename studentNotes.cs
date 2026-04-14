using System;
using System.CodeDom;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Text;
using FilesPath = System.IO.Path;

namespace studentNotes;

public class App : Application
{
    // Colors
    static Color DarkGray       = Color.FromArgb(0xff, 0x26, 0x26, 0x26);
    static Color SemiDarkGray   = Color.FromArgb(0xff, 0x30, 0x30, 0x30);
    static Color DarkerGray     = Color.FromArgb(0xff, 0x1a, 0x1a, 0x1a);
    static Color DarkerAlfa     = Color.FromArgb(0x99, 0x1a, 0x1a, 0x1a);
    static Color DarkerAlfaT    = Color.FromArgb(0x00, 0x1a, 0x1a, 0x1a);
    static Color LightGraya     = Color.FromArgb(0xff, 0xaa, 0xaa, 0xaa);
    static Color LightGrayb     = Color.FromArgb(0xff, 0xaa, 0xaa, 0xaa);
    static Color NeonOrangea    = Color.FromArgb(0xff, 0xff, 0x50, 0x00);
    static Color NeonOrangeb    = Color.FromArgb(0xff, 0xf0, 0x46, 0x00);

    static Color LeftBarCurrent = SemiDarkGray;
    static Color LeftBarDefault = DarkerGray;
    //static Color NeonOrange = tools.CFromARGBint(0xff, 0xff, 0x50, 0x00);

    const int WIN_HEIGHT = 720;
    const int WIN_WIDTH = 1280;

    // Foundation fields \/
    static Application app          = new App();
    static Window root              = new Window();
    static GridEl mainGrid          = new GridEl(WIN_WIDTH+200, WIN_HEIGHT, 3, 5, new SolidColorBrush(DarkGray), HorizontalAlignment.Stretch, VerticalAlignment.Stretch);
    static GridEl LeftBar           = new GridEl(250, WIN_HEIGHT, 6, 2, new SolidColorBrush(NeonOrangea), HorizontalAlignment.Left, VerticalAlignment.Stretch);
    static GridEl Darken            = new GridEl(WIN_WIDTH, WIN_HEIGHT, 1, 1, new SolidColorBrush(DarkerAlfa), HorizontalAlignment.Left, VerticalAlignment.Top);

    static GridEl welcomePage       = new GridEl(WIN_WIDTH, WIN_HEIGHT, 6, 5, new SolidColorBrush(DarkGray), HorizontalAlignment.Left, VerticalAlignment.Top);
    static GridEl plansPage         = new GridEl(WIN_WIDTH, WIN_HEIGHT, 5, 5, new SolidColorBrush(DarkGray), HorizontalAlignment.Left, VerticalAlignment.Top);
    static GridEl notesPage         = new GridEl(WIN_WIDTH, WIN_HEIGHT, 5, 5, new SolidColorBrush(DarkGray), HorizontalAlignment.Left, VerticalAlignment.Top);

    static GridEl toolbar           = new GridEl(440, 80, 1, 3, new SolidColorBrush(SemiDarkGray), HorizontalAlignment.Center, VerticalAlignment.Center);

    // Elements
    static nButton menu_button      = new nButton(50,50,new SolidColorBrush(NeonOrangeb),LeftBarMove,"≡");

    static nButton welcome          = new nButton(250,50, new SolidColorBrush(LeftBarDefault),OnWelcome,"Welcome");
    static nButton plans            = new nButton(250,50, new SolidColorBrush(LeftBarDefault),OnPlans,"Plans");
    static nButton notes            = new nButton(250,50, new SolidColorBrush(LeftBarDefault),OnNotes,"Notes");
    static nButton undarken         = new nButton(WIN_WIDTH, WIN_HEIGHT, new SolidColorBrush(Colors.Transparent),LeftBarMove,"");

    static ListEl fileList          = new ListEl(440,380,new SolidColorBrush(SemiDarkGray), 50);
    static TextB filenamer          = new TextB(240, 0, "foobar",false, 20);
    static TextB fileEdit           = new TextB(450,340, "You shouldn't see this", false, 18, true);
    static nButton filesaver        = new nButton(80,80,new SolidColorBrush(DarkerGray),SaveFile,"Save");
    static DrawRect textBack        = new DrawRect(490, 380, SemiDarkGray);
    static DrawRect toolBack        = new DrawRect(490, 60, SemiDarkGray);
    static DrawRect listBack        = new DrawRect(490, 380, SemiDarkGray);
    static DrawRect menuBack        = new DrawRect(490, 60, SemiDarkGray);
    static Text welcome_notecount   = new Text($"", true, 60);
    static Text welcome_noteabove   = new Text("Current Number of Notes", true, 30);
    static Text welcometext         = new Text("Welcome", true, 100);
    static DropdownList dpl         = new DropdownList(490, 60, new SolidColorBrush(Colors.White), 20, ["Software engineering", "Mathematical Analysis", "Programming II"],[Select_a, Select_b, Select_c]);

    static string notes_pref        = "Software_engineering_";

    static int currentView          = 0;
    static bool welcome_state       = true;
    static bool plans_state         = false;
    static bool notes_state         = false;

    static bool LeftBarState        = false;
    static string currentDirectory  = "";
    static string[] noteFiles       = [];
    
    [STAThread]
    public static void Main()
    {
        root.Title = "Student Notes Project";
        root.MaxWidth = WIN_WIDTH;
        root.MaxHeight = WIN_HEIGHT;
        
        root.Background = new SolidColorBrush(Colors.HotPink);

        root.ResizeMode = ResizeMode.CanResize;
        
        // ------------------------------------

        menu_button.SetGridPos(1,0);
        menu_button.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);

        welcome.SetGridPos(0,1);
        welcome.SetGridColumnSpan(2);
        welcome.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
        welcome.Visibility = Visibility.Hidden;

        plans.SetGridPos(0,2);
        plans.SetGridColumnSpan(2);
        plans.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
        plans.Visibility = Visibility.Hidden;

        notes.SetGridPos(0,3);
        notes.SetGridColumnSpan(2);
        notes.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
        notes.Visibility = Visibility.Hidden;

        LeftBar.SetColumnMaxWidth(0, 200);
        LeftBar.SetColumnMaxWidth(1, 50);
        LeftBar.SetRowMaxHeight(0,50);
        LeftBar.SetRowMaxHeight(1,50);
        LeftBar.SetRowMaxHeight(2,50);
        LeftBar.SetRowMaxHeight(3,50);
        LeftBar.SetRowMaxHeight(4,WIN_HEIGHT - 250);
        LeftBar.SetRowMaxHeight(5,50);
        LeftBar.SetOffset(-200,0);
        LeftBar.AddChild([menu_button.Inherit, welcome.Inherit, plans.Inherit, notes.Inherit]);
        LeftBar.SetGridPos(0,0);
        LeftBar.SetGridSpan(2,3);
        LeftBar.SetZIndex(100);
        //LeftBar.SetAnim(LeftBarMove);

        Darken.SetGridPos(2,0);
        Darken.SetGridSpan(3,3);
        Darken.Visibility = Visibility.Hidden;
        Darken.AddChild(undarken.Inherit);
        Darken.SetZIndex(100);

        Console.WriteLine(FilesPath.GetDirectoryName(System.Environment.ProcessPath));
        Directory.SetCurrentDirectory(FilesPath.GetDirectoryName(System.Environment.ProcessPath));
        currentDirectory = Directory.GetCurrentDirectory();
        //Directory.GetFiles(Directory.GetCurrentDirectory());
        if (!Directory.GetDirectories(currentDirectory).Contains("Notes"))
        {
            Directory.CreateDirectory(currentDirectory + "\\Notes");
        }
        noteFiles = Directory.GetFiles(currentDirectory + "\\Notes");

        welcomePage.SetGridPos(1,0);
        welcomePage.SetGridSpan(4,3);
        welcomePage.SetZIndex(1);
        // welcome Form
        welcomePage.SetColumnMaxWidth(0,190);
        welcomePage.SetColumnMaxWidth(1,360);
        welcomePage.SetColumnMaxWidth(2,130);
        welcomePage.SetColumnMaxWidth(3,360);
        welcomePage.SetColumnMaxWidth(4,190);

        welcomePage.SetRowMaxHeight(0,150);
        welcomePage.SetRowMaxHeight(1,150);
        welcomePage.SetRowMaxHeight(2, 50);
        welcomePage.SetRowMaxHeight(3,100);
        welcomePage.SetRowMaxHeight(4,200);
        welcomePage.SetRowMaxHeight(5,70);

        
        SolidColorBrush welcomeForeground = new SolidColorBrush(Colors.White);
        welcometext.SetGridPos(1,1);
        welcometext.SetGridSpan(3,1);
        welcometext.SetGridAlign(HorizontalAlignment.Center, VerticalAlignment.Top);
        welcometext.SetForeground(welcomeForeground);

        
        welcome_noteabove.SetGridPos(1,3);
        welcome_noteabove.SetGridAlign(HorizontalAlignment.Center, VerticalAlignment.Top);
        welcome_noteabove.SetForeground(welcomeForeground);
        
        welcome_notecount.SetContent($"{noteFiles.Length}");
        welcome_notecount.SetGridPos(1,4);
        welcome_notecount.SetGridAlign(HorizontalAlignment.Center, VerticalAlignment.Top);
        welcome_notecount.SetForeground(welcomeForeground);
        //welcometext.SetBackground(new SolidColorBrush(Colors.HotPink));  // was here for debugging a... bug...
        /*for(int x = 0; x < 25; x += 2)
        {
            GridEl deb = new GridEl(WIN_WIDTH, WIN_HEIGHT, 1, 1, new SolidColorBrush(DarkerAlfa), HorizontalAlignment.Left, VerticalAlignment.Top);
            deb.SetGridPos(x%5,(int)x/5);
            deb.SetGridSpan(1,1);
            welcomePage.AddChild(deb.GetGrid());
        }*/  // Also here for debugging purposes
        //welcomePage.AddChild([welcometext.Inherit, welcome_noteabove.Inherit, welcome_notecount.Inherit]);
        

        plansPage.SetGridPos(1,0);
        plansPage.SetGridSpan(4,3);
        plansPage.SetZIndex(1);

        Text unimp = new Text("UNIMPLEMENTED", true, 100);
        unimp.SetGridPos(0,0);
        unimp.SetGridSpan(5,5);
        unimp.SetForeground(new SolidColorBrush(Colors.White));

        plansPage.AddChild(unimp.Inherit);

        notesPage.SetGridPos(1,0);
        notesPage.SetGridSpan(4,3);
        notesPage.SetZIndex(1);

        notesPage.SetColumnMaxWidth(0,100);
        notesPage.SetColumnMaxWidth(1,490);
        notesPage.SetColumnMaxWidth(2,100);
        notesPage.SetColumnMaxWidth(3,490);
        notesPage.SetColumnMaxWidth(4,100);

        notesPage.SetRowMaxHeight(0,100);
        notesPage.SetRowMaxHeight(1, 60);
        notesPage.SetRowMaxHeight(2, 50);
        notesPage.SetRowMaxHeight(3,380);
        notesPage.SetRowMaxHeight(4,100);
        //notesPage.ShowGrid(); // debug

        listBack.SetGridPos(1,3);
        listBack.SetGridSpan(1,1);

        toolBack.SetGridPos(3,1);
        toolBack.SetGridSpan(1,1);

        textBack.SetGridPos(3,3);
        textBack.SetGridSpan(1,1);

        menuBack.SetGridPos(1,1);

        toolbar.SetGridPos(3,1);
        toolbar.SetColumnMaxWidth(0, 90);
        toolbar.SetColumnMaxWidth(1, 260);
        toolbar.SetColumnMaxWidth(2, 90);

        Text filelabel = new Text("Filename:", true, 20);
        filelabel.SetGridPos(0,0);
        filelabel.SetForeground(new SolidColorBrush(Colors.White));
        //filelabel.SetBackground(new SolidColorBrush(DarkerGray));

        filenamer.SetGridPos(1,0);
        filenamer.SetForeground(new SolidColorBrush(Colors.White));
        filenamer.SetBackground(new SolidColorBrush(DarkerGray));

        filesaver.SetGridPos(2,0);

        toolbar.AddChild([filelabel.Inherit, filenamer.Inherit, filesaver.Inherit]);

        fileEdit.SetGridPos(3,3);
        fileEdit.SetForeground(new SolidColorBrush(Colors.White));
        fileEdit.SetBackground(new SolidColorBrush(DarkerGray));

        dpl.SetGridPos(1,1);
        dpl.SetForeground(new SolidColorBrush(Colors.White));

        ReconstructFileList();
        //FileListItem l0 = new FileListItem(400,45,new SolidColorBrush(DarkerGray),(object sender, RoutedEventArgs e) => {Console.WriteLine($"load: {((Button)sender).Content}");},(object sender, RoutedEventArgs e) => {Console.WriteLine($"delete: {((Button)sender).Content}");},new SolidColorBrush(DarkerGray), new SolidColorBrush(DarkGray),new SolidColorBrush(Colors.White), "foo");
        //FileListItem l1 = new FileListItem(400,45,new SolidColorBrush(DarkerGray),(object sender, RoutedEventArgs e) => {Console.WriteLine($"load: {((Button)sender).Content}");},(object sender, RoutedEventArgs e) => {Console.WriteLine($"delete: {((Button)sender).Content}");},new SolidColorBrush(DarkerGray), new SolidColorBrush(DarkGray),new SolidColorBrush(Colors.White),"bar");
        //fileList.AddItem(l0);
        //fileList.AddItem(l1);

        //Console.WriteLine(fileList.Len()); // debug

        //notesPage.AddChild([listBack.Inherit, textBack.Inherit, toolBack.Inherit, fileList.Inherit, fileEdit.Inherit]);
        //notesPage.AddChild(toolbar.GetGrid());


        mainGrid.SetColumnMaxWidth(0, 50);
        mainGrid.SetColumnMaxWidth(1, 200);
        mainGrid.SetColumnMaxWidth(4, 200);
        mainGrid.AddChild([LeftBar.GetGrid(), Darken.GetGrid(), welcomePage.GetGrid(), plansPage.GetGrid(), notesPage.GetGrid()]);

        ResolveCurrentView();

        root.Content = mainGrid.GetGrid();
        root.Show();
        app.MainWindow = root;
        app.Run();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
    }

    public static void menu_buttonon_call(object sender, RoutedEventArgs e)
    {
        
    }

    public static void LeftBarMove(object sender, RoutedEventArgs e)
    {
        LeftBarState = !LeftBarState;
        Duration duration = new Duration(TimeSpan.FromMilliseconds(200));
        LinearMatrixAnimation lma = new LinearMatrixAnimation(new Matrix(1,0,0,1,LeftBarState ? -200 : 0,0), new Matrix(1,0,0,1,LeftBarState ? 0 : -200,0), duration);
        ColorAnimation ca;
        ColorAnimation ca2;
        ColorAnimation ca3;
        if(LeftBarState)
        {
            ca = new ColorAnimation(DarkerAlfaT, DarkerAlfa, duration);
            ca2 = new ColorAnimation(NeonOrangea, LeftBarCurrent, duration);
            ca3 = new ColorAnimation(NeonOrangea, LeftBarDefault, duration);
        }
        else
        {
            ca = new ColorAnimation(DarkerAlfa, DarkerAlfaT, duration);
            ca2 = new ColorAnimation(LeftBarCurrent, NeonOrangea, duration);
            ca3 = new ColorAnimation(LeftBarDefault, NeonOrangea, duration);
            ca.Completed += (object? sender, EventArgs e)=>{Darken.Visibility = Visibility.Hidden;};
            ca2.Completed += (object? sender, EventArgs e)=>{welcome.Visibility = Visibility.Hidden; plans.Visibility = Visibility.Hidden; notes.Visibility = Visibility.Hidden;};
        }
        QuadraticEase easefoo = new QuadraticEase();
        easefoo.EasingMode = EasingMode.EaseInOut;
        lma.EasingFunction = easefoo;
        ca.EasingFunction = easefoo;
        Console.WriteLine("Button Clicked!");
        LeftBar.AnimTransform(sender, e, lma); // Don't know why it doesn't work, propably broken MatrixProperty or something 
        Darken.AnimTransform(sender, e, lma); // Don't know why it doesn't work, propably broken MatrixProperty or something 
        Darken.AnimColor(sender, e, ca);
            welcome.AnimColor(sender, e, welcome_state ? ca2 : ca3);
            plans.AnimColor(sender, e, plans_state ? ca2 : ca3);
            notes.AnimColor(sender, e, notes_state ? ca2 : ca3);
        //LeftBar.SetOffset(LeftBarState ? 0 : -200,0);
        //Darken.SetOffset(LeftBarState ? 0 : -200,0);
        if (LeftBarState)
        {
            Darken.Visibility = Visibility.Visible;
            welcome.Visibility = Visibility.Visible;
            plans.Visibility = Visibility.Visible;
            notes.Visibility = Visibility.Visible;
        }
    }

    public static void OnWelcome(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Welcome!");
        currentView = 0;
        ResolveCurrentView();
    }

    public static void OnPlans(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Plans!");
        currentView = 1;
        ResolveCurrentView();
    }

    public static void OnNotes(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Notes!");
        currentView = 2;
        ResolveCurrentView();
    }

    public static void ResolveCurrentView()
    {
        SolidColorBrush cur = new SolidColorBrush(LeftBarCurrent);
        SolidColorBrush def = new SolidColorBrush(LeftBarDefault);
        switch (currentView)
        {
            case 0:
                {
                    welcomePage.SetZIndex(2);
                    welcome.SetBackground(cur);
                    welcome_state = true;
                    plansPage.SetZIndex(1);
                    plans.SetBackground(def);
                    plans_state = false;
                    notesPage.SetZIndex(1);
                    notes.SetBackground(def);
                    notes_state = false;
                    break;
                }
            case 1:
                {
                    welcomePage.SetZIndex(1);
                    welcome.SetBackground(def);
                    welcome_state = false;
                    plansPage.SetZIndex(2);
                    plans.SetBackground(cur);
                    plans_state = true;
                    notesPage.SetZIndex(1);
                    notes.SetBackground(def);
                    notes_state = false;
                    break;
                }
            case 2:
                {
                    welcomePage.SetZIndex(1);
                    welcome.SetBackground(def);
                    welcome_state = false;
                    plansPage.SetZIndex(1);
                    plans.SetBackground(def);
                    plans_state = false;
                    notesPage.SetZIndex(2);
                    notes.SetBackground(cur);
                    notes_state = true;
                    break;
                }
            default:
                {
                    Console.WriteLine($"current view error, value beyound 1-2, found: {currentView}");
                    break;
                }
        }
    }

    public static void DeleteFile(object sender, RoutedEventArgs e)
    {
        if(File.Exists(currentDirectory +  "\\Notes\\" + notes_pref + ((Button)sender).DataContext + ".txt"))
        {
            File.Delete(currentDirectory +  "\\Notes\\" + notes_pref + ((Button)sender).DataContext + ".txt");
            ReconstructFileList();
        }
        else
        {
            throw new Exception();
        }
    }

    public static void LoadFile(object sender, RoutedEventArgs e)
    {
        if(!File.Exists(currentDirectory +  "\\Notes\\" + notes_pref + ((Button)sender).DataContext + ".txt"))
        {
            Console.WriteLine($"Filename: {currentDirectory + "\\Notes\\" + notes_pref + ((Button)sender).DataContext + ".txt"}");
            throw new Exception();
        }
        string data = File.ReadAllText(currentDirectory +  "\\Notes\\" + notes_pref + ((Button)sender).DataContext + ".txt");
        fileEdit.SetContent(data);
        filenamer.SetContent((string)((Button)sender).DataContext);
    }

    public static void SaveFile(object sender, RoutedEventArgs e)
    {
        //Console.WriteLine(currentDirectory + "\\Notes\\" + filenamer.GetContent() + ".txt");
        File.WriteAllText(currentDirectory + "\\Notes\\" + notes_pref + filenamer.GetContent() + ".txt", fileEdit.GetContent());
        //Console.WriteLine(fileEdit.GetContent());
        ReconstructFileList();
    }

    public static void ReconstructFileList()
    {
        fileList = new ListEl(440,380,new SolidColorBrush(SemiDarkGray), 50);
        fileList.SetGridPos(1,3);
        fileList.SetGridSpan(1,1);
        fileList.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
        noteFiles = Directory.GetFiles(currentDirectory + "\\Notes");
        for(int i = 0; i < noteFiles.Length; i++)
        {
            string temp = FilesPath.GetFileNameWithoutExtension(noteFiles[i]);
            if(temp.StartsWith(notes_pref)){
                temp = temp.Replace(notes_pref, "");
                fileList.AddItem(new FileListItem(400,45,new SolidColorBrush(DarkerGray),LoadFile,DeleteFile,new SolidColorBrush(DarkerGray), new SolidColorBrush(DarkGray),new SolidColorBrush(Colors.White), temp));
            }
        }
        notesPage.ResetChildren();
        notesPage.AddChild([listBack.Inherit, textBack.Inherit, toolBack.Inherit, menuBack.Inherit, dpl.Inherit, fileList.Inherit, fileEdit.Inherit]);
        notesPage.AddChild(toolbar.GetGrid());

        welcomePage.ResetChildren();
        SolidColorBrush welcomeForeground = new SolidColorBrush(Colors.White);
        welcome_notecount = new Text($"{noteFiles.Length}", true, 60);
        welcome_notecount.SetGridPos(1,4);
        welcome_notecount.SetGridAlign(HorizontalAlignment.Center, VerticalAlignment.Top);
        welcome_notecount.SetForeground(welcomeForeground);
        welcomePage.AddChild([welcometext.Inherit, welcome_noteabove.Inherit, welcome_notecount.Inherit]);
    }

    public static void Select_a(object sender, RoutedEventArgs e)
    {
        notes_pref = "Software_engineering_";
        ReconstructFileList();
    }

    public static void Select_b(object sender, RoutedEventArgs e)
    {
        notes_pref = "Mathematical_Analysis_";
        ReconstructFileList();
    }

    public static void Select_c(object sender, RoutedEventArgs e)
    {
        notes_pref = "Programming_II_";
        ReconstructFileList();
    }
}