#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using WillExe;
using WillExe.Viewers;

namespace WillExe.EditorTools
{
    public static class WillExeSceneBuilder
    {
        static readonly Color WallpaperTeal = new Color(0.016f, 0.502f, 0.502f);
        static readonly Color Win98Gray = new Color(0.753f, 0.753f, 0.753f);
        static readonly Color TitlebarNavy = new Color(0.0f, 0.0f, 0.502f);
        static readonly Color TitlebarText = Color.white;
        static readonly Color BodyWhite = Color.white;
        static readonly Color TextDark = Color.black;

        const string PrefabDir = "Assets/Prefabs";
        const string ViewerDir = "Assets/Prefabs/Viewers";
        const string ContentDir = "Assets/Content";
        const string GeneratedDir = "Assets/Art/Generated";
        const string IconsDir = "Assets/Art/Icons";
        const string RickrollDir = "Assets/Art/Rickroll";

        static Sprite LoadSpriteAt(string path, FilterMode filter = FilterMode.Bilinear)
        {
            if (!File.Exists(path)) return null;
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && (importer.textureType != TextureImporterType.Sprite || importer.filterMode != filter))
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.mipmapEnabled = false;
                importer.filterMode = filter;
                if (filter == FilterMode.Point) importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        static Sprite LoadGeneratedSprite(string fileName) => LoadSpriteAt($"{GeneratedDir}/{fileName}");
        static Sprite LoadIcon(string fileName) => LoadSpriteAt($"{IconsDir}/{fileName}", FilterMode.Point);

        static Sprite[] LoadRickrollFrames()
        {
            if (!AssetDatabase.IsValidFolder(RickrollDir)) return new Sprite[0];
            var files = Directory.GetFiles(RickrollDir, "*.png");
            System.Array.Sort(files);
            var list = new System.Collections.Generic.List<Sprite>(files.Length);
            foreach (var f in files)
            {
                var s = LoadSpriteAt(f.Replace('\\', '/'));
                if (s != null) list.Add(s);
            }
            return list.ToArray();
        }

        // ---------- Utility helpers ----------
        static GameObject NewUI(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        static RectTransform Fit(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = (RectTransform)go.transform;
            rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
            return rt;
        }

        static RectTransform Stretch(GameObject go) => Fit(go, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        static Image AddImage(GameObject go, Color c)
        {
            var img = go.AddComponent<Image>();
            img.color = c;
            return img;
        }

        static TMP_Text AddText(GameObject go, string text, int size, Color c, TextAlignmentOptions align = TextAlignmentOptions.MidlineLeft)
        {
            var t = go.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = size;
            t.color = c;
            t.alignment = align;
            t.enableWordWrapping = true;
            return t;
        }

        static void SetPos(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchored, Vector2 size)
        {
            var rt = (RectTransform)go.transform;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.sizeDelta = size;
            rt.anchoredPosition = anchored;
        }

        // ---------- Boot scene ----------
        [MenuItem("WillExe/1. Build Boot Scene")]
        public static void BuildBoot()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var canvasGO = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            var bg = NewUI("Background", canvasGO.transform);
            Stretch(bg);
            var bgImg = AddImage(bg, new Color(0.04f, 0.08f, 0.04f));
            var bootBgSprite = LoadGeneratedSprite("boot_background.png");
            if (bootBgSprite != null)
            {
                bgImg.sprite = bootBgSprite;
                bgImg.color = Color.white;
                bgImg.preserveAspect = false;
            }
            var scrim = NewUI("Scrim", canvasGO.transform);
            Stretch(scrim);
            AddImage(scrim, new Color(0, 0, 0, 0.72f));

            // Title
            var title = NewUI("Title", canvasGO.transform);
            SetPos(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -210), new Vector2(1200, 150));
            var titleTxt = AddText(title, "WILL.EXE", 130, Color.white, TextAlignmentOptions.Center);
            titleTxt.fontStyle = FontStyles.Bold;

            var subtitle = NewUI("Subtitle", canvasGO.transform);
            SetPos(subtitle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -320), new Vector2(1200, 36));
            AddText(subtitle, "v1.0 :: System Inheritance Protocol", 28, new Color(0.8f, 0.8f, 0.8f), TextAlignmentOptions.Center);

            var tagline = NewUI("Tagline", canvasGO.transform);
            SetPos(tagline, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 80), new Vector2(1000, 200));
            var taglineText = AddText(tagline,
                "Your grandfather is dead. He left you his PC, and everything else.\n\n" +
                "Crack 4 passwords. Assemble the will. Inherit the empire.",
                30, new Color(0.9f, 0.9f, 0.9f), TextAlignmentOptions.Center);
            taglineText.lineSpacing = 6f;

            // Difficulty section — bottom-left corner, stacked vertically
            var diffLabelGO = NewUI("DifficultyLabel", canvasGO.transform);
            SetPos(diffLabelGO, new Vector2(0, 0), new Vector2(0, 0), new Vector2(70, 260), new Vector2(400, 40));
            var diffLabelRt = (RectTransform)diffLabelGO.transform;
            diffLabelRt.pivot = new Vector2(0, 0);
            var diffLabel = AddText(diffLabelGO, "Difficulty", 24, new Color(0.7f, 0.7f, 0.7f), TextAlignmentOptions.Left);
            diffLabel.fontStyle = FontStyles.Bold;

            var radioRow = NewUI("DifficultyRadios", canvasGO.transform);
            SetPos(radioRow, new Vector2(0, 0), new Vector2(0, 0), new Vector2(40, 60), new Vector2(600, 180));
            var radioRt = (RectTransform)radioRow.transform;
            radioRt.pivot = new Vector2(0, 0);
            var radioVLG = radioRow.AddComponent<VerticalLayoutGroup>();
            radioVLG.spacing = 12;
            radioVLG.childAlignment = TextAnchor.UpperLeft;
            radioVLG.childControlWidth = true; radioVLG.childControlHeight = true;
            radioVLG.childForceExpandWidth = true; radioVLG.childForceExpandHeight = false;
            var easyBtn = MakeRadioOption("Easy  -  No time limit", radioRow.transform, out var easyText);
            var normalBtn = MakeRadioOption("Normal  -  15 min", radioRow.transform, out var normalText);
            var hardBtn = MakeRadioOption("Hard  -  8 min + penalty", radioRow.transform, out var hardText);

            // Hidden legacy dropdown (kept null-safe, BootMenu pills take over)
            var ddGO = NewUI("DifficultyDropdown", canvasGO.transform);
            ddGO.SetActive(false);
            SetPos(ddGO, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(420, 60));
            var ddImg = AddImage(ddGO, Win98Gray);
            var dropdown = ddGO.AddComponent<TMP_Dropdown>();
            // Dropdown label
            var labelGO = NewUI("Label", ddGO.transform);
            Fit(labelGO, Vector2.zero, Vector2.one, new Vector2(16, 0), new Vector2(-40, 0));
            var labelText = AddText(labelGO, "NORMAL", 32, TextDark, TextAlignmentOptions.MidlineLeft);
            dropdown.captionText = (TextMeshProUGUI)labelText;
            // Template (required)
            var templateGO = NewUI("Template", ddGO.transform);
            SetPos(templateGO, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 180));
            var templateRt = (RectTransform)templateGO.transform;
            templateRt.pivot = new Vector2(0.5f, 1);
            AddImage(templateGO, Win98Gray);
            templateGO.SetActive(false);
            // Viewport
            var viewportGO = NewUI("Viewport", templateGO.transform);
            Stretch(viewportGO);
            AddImage(viewportGO, Win98Gray);
            var mask = viewportGO.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            // Content
            var contentGO = NewUI("Content", viewportGO.transform);
            SetPos(contentGO, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 180));
            var contentRt = (RectTransform)contentGO.transform;
            contentRt.pivot = new Vector2(0.5f, 1);
            // Item
            var itemGO = NewUI("Item", contentGO.transform);
            SetPos(itemGO, new Vector2(0, 0.5f), new Vector2(1, 0.5f), Vector2.zero, new Vector2(0, 50));
            var itemToggle = itemGO.AddComponent<Toggle>();
            var itemBG = NewUI("Item Background", itemGO.transform);
            Stretch(itemBG);
            var itemBgImg = AddImage(itemBG, Win98Gray);
            var itemCheckmark = NewUI("Item Checkmark", itemGO.transform);
            SetPos(itemCheckmark, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(16, 0), new Vector2(24, 24));
            var checkImg = AddImage(itemCheckmark, TitlebarNavy);
            var itemLabelGO = NewUI("Item Label", itemGO.transform);
            Fit(itemLabelGO, Vector2.zero, Vector2.one, new Vector2(44, 0), new Vector2(-8, 0));
            var itemLabelTxt = AddText(itemLabelGO, "Option", 28, TextDark, TextAlignmentOptions.MidlineLeft);
            itemToggle.targetGraphic = itemBgImg;
            itemToggle.graphic = checkImg;
            dropdown.template = templateRt;
            dropdown.itemText = (TextMeshProUGUI)itemLabelTxt;
            dropdown.targetGraphic = ddImg;

            // Boot button — centered below tagline
            var btnGO = NewUI("BootButton", canvasGO.transform);
            SetPos(btnGO, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -140), new Vector2(380, 80));
            var btnImg = AddImage(btnGO, Color.white);
            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = btnImg;
            var btnColors = btn.colors;
            btnColors.highlightedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
            btnColors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            btn.colors = btnColors;
            var btnLabelGO = NewUI("Label", btnGO.transform);
            Stretch(btnLabelGO);
            var btnLabel = AddText(btnLabelGO, "Boot System", 32, Color.black, TextAlignmentOptions.Center);
            btnLabel.fontStyle = FontStyles.Bold;

            // BootMenu script on Canvas
            var bootMenu = canvasGO.AddComponent<BootMenu>();
            bootMenu.difficultyDropdown = dropdown;
            bootMenu.bootButton = btn;
            bootMenu.tagline = (TMP_Text)tagline.GetComponent<TMP_Text>();
            bootMenu.easyButton = easyBtn;
            bootMenu.normalButton = normalBtn;
            bootMenu.hardButton = hardBtn;
            bootMenu.easyText = easyText;
            bootMenu.normalText = normalText;
            bootMenu.hardText = hardText;

            // AudioOneShot
            var audioGO = new GameObject("AudioOneShot");
            audioGO.AddComponent<AudioOneShot>();

            // EventSystem + Camera
            MakeEventSystem();
            MakeMainCamera();

            EnsureScenesFolder();
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Boot.unity");
            Debug.Log("[WillExe] Boot scene built.");
        }

        static void EnsureScenesFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        // ---------- Prefabs ----------
        [MenuItem("WillExe/2. Build Prefabs")]
        public static void BuildPrefabs()
        {
            if (!AssetDatabase.IsValidFolder(PrefabDir)) AssetDatabase.CreateFolder("Assets", "Prefabs");
            if (!AssetDatabase.IsValidFolder(ViewerDir)) AssetDatabase.CreateFolder(PrefabDir, "Viewers");

            BuildDesktopWindowPrefab();
            BuildDesktopIconPrefab();
            BuildTextViewerPrefab();
            BuildImageViewerPrefab();
            BuildFolderViewerPrefab();
            BuildPasswordGatePrefab();
            BuildVideoViewerPrefab();
            BuildWillFragmentViewerPrefab();

            AssetDatabase.SaveAssets();
            Debug.Log("[WillExe] Prefabs built.");
        }

        static void BuildDesktopWindowPrefab()
        {
            var root = new GameObject("DesktopWindow", typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)root.transform;
            rt.sizeDelta = new Vector2(640, 420);
            var bg = root.GetComponent<Image>();
            bg.color = Win98Gray;

            // Titlebar
            var titleBar = NewUI("TitleBar", root.transform);
            SetPos(titleBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -24), new Vector2(0, 48));
            var tbRt = (RectTransform)titleBar.transform;
            tbRt.anchorMin = new Vector2(0, 1); tbRt.anchorMax = new Vector2(1, 1);
            tbRt.pivot = new Vector2(0.5f, 1f);
            tbRt.offsetMin = new Vector2(4, -48);
            tbRt.offsetMax = new Vector2(-4, -4);
            AddImage(titleBar, TitlebarNavy);

            var titleLabelGO = NewUI("TitleLabel", titleBar.transform);
            Fit(titleLabelGO, Vector2.zero, Vector2.one, new Vector2(12, 0), new Vector2(-60, 0));
            var titleLabel = AddText(titleLabelGO, "Window", 28, TitlebarText, TextAlignmentOptions.MidlineLeft);

            // Close button
            var closeGO = NewUI("CloseButton", titleBar.transform);
            var closeRt = (RectTransform)closeGO.transform;
            closeRt.anchorMin = new Vector2(1, 0.5f);
            closeRt.anchorMax = new Vector2(1, 0.5f);
            closeRt.pivot = new Vector2(1, 0.5f);
            closeRt.sizeDelta = new Vector2(44, 36);
            closeRt.anchoredPosition = new Vector2(-4, 0);
            var closeImg = AddImage(closeGO, new Color(0.80f, 0.15f, 0.15f));
            var closeBtn = closeGO.AddComponent<Button>();
            closeBtn.targetGraphic = closeImg;
            var xGO = NewUI("X", closeGO.transform);
            Stretch(xGO);
            var xText = AddText(xGO, "X", 28, Color.white, TextAlignmentOptions.Center);
            xText.fontStyle = FontStyles.Bold;

            // Content slot
            var contentSlot = NewUI("ContentSlot", root.transform);
            Fit(contentSlot, Vector2.zero, Vector2.one, new Vector2(4, 4), new Vector2(-4, -52));
            AddImage(contentSlot, BodyWhite);

            var wc = root.AddComponent<WindowController>();
            wc.titleBar = tbRt;
            wc.titleLabel = titleLabel;
            wc.closeButton = closeBtn;
            wc.contentSlot = contentSlot.transform;

            PrefabUtility.SaveAsPrefabAsset(root, $"{PrefabDir}/DesktopWindow.prefab");
            Object.DestroyImmediate(root);
        }

        static void BuildDesktopIconPrefab()
        {
            var root = new GameObject("DesktopIcon", typeof(RectTransform));
            var rt = (RectTransform)root.transform;
            rt.sizeDelta = new Vector2(110, 130);

            var iconGO = NewUI("Icon", root.transform);
            SetPos(iconGO, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -48), new Vector2(80, 80));
            var img = AddImage(iconGO, new Color(0.45f, 0.55f, 0.75f));
            img.preserveAspect = true;

            var labelGO = NewUI("Label", root.transform);
            SetPos(labelGO, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 18), new Vector2(104, 44));
            var label = AddText(labelGO, "Label", 15, new Color(0.08f, 0.08f, 0.1f), TextAlignmentOptions.Center);
            label.fontStyle = FontStyles.Bold;
            label.textWrappingMode = TextWrappingModes.Normal;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.enableAutoSizing = false;
            var labelOutline = labelGO.AddComponent<UnityEngine.UI.Outline>();
            labelOutline.effectColor = new Color(1, 1, 1, 0.95f);
            labelOutline.effectDistance = new Vector2(1f, -1f);

            var ib = root.AddComponent<IconBehavior>();
            ib.iconImage = img;
            ib.labelText = label;

            PrefabUtility.SaveAsPrefabAsset(root, $"{PrefabDir}/DesktopIcon.prefab");
            Object.DestroyImmediate(root);
        }

        static void BuildTextViewerPrefab()
        {
            var root = new GameObject("TextViewer", typeof(RectTransform));
            Stretch(root);
            var scrollGO = NewUI("Scroll", root.transform);
            Stretch(scrollGO);
            AddImage(scrollGO, BodyWhite);
            var scroll = scrollGO.AddComponent<ScrollRect>();
            scroll.horizontal = false;

            var viewportGO = NewUI("Viewport", scrollGO.transform);
            Fit(viewportGO, Vector2.zero, Vector2.one, new Vector2(12, 12), new Vector2(-20, -12));
            var viewImg = AddImage(viewportGO, BodyWhite);
            viewportGO.AddComponent<Mask>().showMaskGraphic = false;

            var contentGO = NewUI("Content", viewportGO.transform);
            var contentRt = (RectTransform)contentGO.transform;
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0.5f, 1);
            contentRt.sizeDelta = new Vector2(0, 400);
            contentRt.anchoredPosition = Vector2.zero;
            var csf = contentGO.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var bodyGO = NewUI("Body", contentGO.transform);
            Stretch(bodyGO);
            var body = AddText(bodyGO, "", 28, TextDark, TextAlignmentOptions.TopLeft);
            body.margin = new Vector4(8, 8, 8, 8);

            scroll.viewport = (RectTransform)viewportGO.transform;
            scroll.content = contentRt;

            var tv = root.AddComponent<TextViewer>();
            tv.body = body;

            PrefabUtility.SaveAsPrefabAsset(root, $"{ViewerDir}/TextViewer.prefab");
            Object.DestroyImmediate(root);
        }

        static void BuildImageViewerPrefab()
        {
            var root = new GameObject("ImageViewer", typeof(RectTransform));
            Stretch(root);
            AddImage(root, BodyWhite);

            var imgGO = NewUI("Picture", root.transform);
            Fit(imgGO, new Vector2(0, 0.2f), new Vector2(1, 1), new Vector2(16, 16), new Vector2(-16, -16));
            var pic = AddImage(imgGO, new Color(0.3f, 0.3f, 0.3f));
            pic.preserveAspect = true;

            var capGO = NewUI("Caption", root.transform);
            Fit(capGO, Vector2.zero, new Vector2(1, 0.22f), new Vector2(16, 8), new Vector2(-16, -4));
            var cap = AddText(capGO, "", 16, TextDark, TextAlignmentOptions.Center);
            cap.textWrappingMode = TextWrappingModes.Normal;
            cap.enableAutoSizing = true;
            cap.fontSizeMin = 11;
            cap.fontSizeMax = 18;

            var iv = root.AddComponent<ImageViewer>();
            iv.picture = pic;
            iv.caption = cap;

            PrefabUtility.SaveAsPrefabAsset(root, $"{ViewerDir}/ImageViewer.prefab");
            Object.DestroyImmediate(root);
        }

        static void BuildFolderViewerPrefab()
        {
            var root = new GameObject("FolderViewer", typeof(RectTransform));
            Stretch(root);
            AddImage(root, BodyWhite);

            var gridGO = NewUI("Grid", root.transform);
            Fit(gridGO, Vector2.zero, Vector2.one, new Vector2(16, 16), new Vector2(-16, -16));
            var grid = gridGO.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(110, 130);
            grid.spacing = new Vector2(12, 12);
            grid.padding = new RectOffset(8, 8, 8, 8);

            var fv = root.AddComponent<FolderViewer>();
            fv.grid = (RectTransform)gridGO.transform;
            // iconPrefab assigned later via binding script

            PrefabUtility.SaveAsPrefabAsset(root, $"{ViewerDir}/FolderViewer.prefab");
            Object.DestroyImmediate(root);
        }

        static void BuildPasswordGatePrefab()
        {
            var root = new GameObject("PasswordGate", typeof(RectTransform));
            Stretch(root);
            AddImage(root, BodyWhite);

            var promptGO = NewUI("Prompt", root.transform);
            SetPos(promptGO, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(580, 180));
            var prompt = AddText(promptGO, "Enter password:", 22, TextDark, TextAlignmentOptions.Center);
            prompt.textWrappingMode = TextWrappingModes.Normal;
            prompt.enableAutoSizing = true;
            prompt.fontSizeMin = 14;
            prompt.fontSizeMax = 26;

            var inputGO = NewUI("Input", root.transform);
            SetPos(inputGO, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40), new Vector2(520, 70));
            var inImg = AddImage(inputGO, new Color(0.95f, 0.95f, 0.95f));
            var input = inputGO.AddComponent<TMP_InputField>();
            input.targetGraphic = inImg;
            // TextArea / Text child
            var textAreaGO = NewUI("TextArea", inputGO.transform);
            Fit(textAreaGO, Vector2.zero, Vector2.one, new Vector2(12, 8), new Vector2(-12, -8));
            textAreaGO.AddComponent<RectMask2D>();
            var placeholderGO = NewUI("Placeholder", textAreaGO.transform);
            Stretch(placeholderGO);
            var placeholder = AddText(placeholderGO, "type here...", 28, new Color(0.5f, 0.5f, 0.5f), TextAlignmentOptions.MidlineLeft);
            var inTextGO = NewUI("Text", textAreaGO.transform);
            Stretch(inTextGO);
            var inText = AddText(inTextGO, "", 28, TextDark, TextAlignmentOptions.MidlineLeft);
            input.textViewport = (RectTransform)textAreaGO.transform;
            input.textComponent = (TextMeshProUGUI)inText;
            input.placeholder = placeholder;

            var submitGO = NewUI("Submit", root.transform);
            SetPos(submitGO, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(280, 70));
            var submitImg = AddImage(submitGO, Win98Gray);
            var submit = submitGO.AddComponent<Button>();
            submit.targetGraphic = submitImg;
            var submitLabelGO = NewUI("Label", submitGO.transform);
            Stretch(submitLabelGO);
            AddText(submitLabelGO, "SUBMIT", 28, TextDark, TextAlignmentOptions.Center);

            var feedGO = NewUI("Feedback", root.transform);
            SetPos(feedGO, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 60), new Vector2(560, 60));
            var feed = AddText(feedGO, "", 26, new Color(0.7f, 0, 0), TextAlignmentOptions.Center);

            var gate = root.AddComponent<PasswordGate>();
            gate.promptLabel = prompt;
            gate.input = input;
            gate.submit = submit;
            gate.feedback = feed;

            PrefabUtility.SaveAsPrefabAsset(root, $"{ViewerDir}/PasswordGate.prefab");
            Object.DestroyImmediate(root);
        }

        static void BuildVideoViewerPrefab()
        {
            var root = new GameObject("VideoViewer", typeof(RectTransform));
            Stretch(root);
            AddImage(root, Color.black);

            var picGO = NewUI("StaticImage", root.transform);
            Stretch(picGO);
            var pic = AddImage(picGO, new Color(0.2f, 0.2f, 0.2f));
            pic.preserveAspect = true;

            var vv = root.AddComponent<VideoViewer>();
            vv.staticImage = pic;

            PrefabUtility.SaveAsPrefabAsset(root, $"{ViewerDir}/VideoViewer.prefab");
            Object.DestroyImmediate(root);
        }

        static void BuildWillFragmentViewerPrefab()
        {
            var root = new GameObject("WillFragmentViewer", typeof(RectTransform));
            Stretch(root);
            AddImage(root, new Color(0.98f, 0.95f, 0.85f));

            var stampGO = NewUI("Stamp", root.transform);
            SetPos(stampGO, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(600, 60));
            var stamp = AddText(stampGO, "WILL FRAGMENT", 28, new Color(0.5f, 0, 0), TextAlignmentOptions.Center);

            var bodyGO = NewUI("Body", root.transform);
            Fit(bodyGO, Vector2.zero, Vector2.one, new Vector2(32, 32), new Vector2(-32, -100));
            var body = AddText(bodyGO, "", 28, TextDark, TextAlignmentOptions.TopLeft);

            var v = root.AddComponent<WillFragmentViewer>();
            v.stamp = stamp;
            v.fragmentBody = body;

            PrefabUtility.SaveAsPrefabAsset(root, $"{ViewerDir}/WillFragmentViewer.prefab");
            Object.DestroyImmediate(root);
        }

        // ---------- Desktop scene ----------
        [MenuItem("WillExe/3. Build Desktop Scene")]
        public static void BuildDesktop()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var canvasGO = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            var wallpaper = NewUI("Wallpaper", canvasGO.transform);
            Stretch(wallpaper);
            var wallpaperImg = AddImage(wallpaper, WallpaperTeal);
            var wallpaperSprite = LoadGeneratedSprite("desktop_wallpaper.png");
            if (wallpaperSprite != null)
            {
                wallpaperImg.sprite = wallpaperSprite;
                wallpaperImg.color = Color.white;
                wallpaperImg.preserveAspect = false;
            }

            var iconLayer = NewUI("IconLayer", canvasGO.transform);
            Stretch(iconLayer);

            var windowLayer = NewUI("WindowLayer", canvasGO.transform);
            Stretch(windowLayer);

            // Taskbar
            var taskbar = NewUI("Taskbar", canvasGO.transform);
            var tbRt = (RectTransform)taskbar.transform;
            tbRt.anchorMin = new Vector2(0, 0);
            tbRt.anchorMax = new Vector2(1, 0);
            tbRt.pivot = new Vector2(0.5f, 0);
            tbRt.sizeDelta = new Vector2(0, 72);
            tbRt.anchoredPosition = Vector2.zero;
            AddImage(taskbar, Win98Gray);

            var startGO = NewUI("StartButton", taskbar.transform);
            SetPos(startGO, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(110, 0), new Vector2(200, 56));
            var startImg = AddImage(startGO, new Color(0.82f, 0.82f, 0.82f));
            var startBtn = startGO.AddComponent<Button>();
            startBtn.targetGraphic = startImg;
            var startIconGO = NewUI("Icon", startGO.transform);
            SetPos(startIconGO, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(32, 0), new Vector2(40, 40));
            var startIconImg = AddImage(startIconGO, Color.white);
            var startIconSprite = LoadIcon("windows-0.png");
            if (startIconSprite != null)
            {
                startIconImg.sprite = startIconSprite;
                startIconImg.preserveAspect = true;
            }
            else
            {
                startIconImg.color = new Color(0.0f, 0.3f, 0.65f);
            }
            var startLabelPad = 64;
            var startLabelGO = NewUI("Label", startGO.transform);
            Fit(startLabelGO, Vector2.zero, Vector2.one, new Vector2(startLabelPad, 0), new Vector2(-8, 0));
            var startLabel = AddText(startLabelGO, "Start", 26, TextDark, TextAlignmentOptions.MidlineLeft);
            startLabel.fontStyle = FontStyles.Bold;

            var fragCounterGO = NewUI("FragmentCounter", taskbar.transform);
            SetPos(fragCounterGO, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-260, 0), new Vector2(480, 56));
            var fragCounter = AddText(fragCounterGO, "WILL FRAGMENTS 0/4", 28, TextDark, TextAlignmentOptions.MidlineRight);

            // Drive Health HUD
            var healthBarBG = NewUI("DriveHealthBG", taskbar.transform);
            SetPos(healthBarBG, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(520, 36));
            AddImage(healthBarBG, new Color(0.2f, 0.2f, 0.2f));
            var healthBarFill = NewUI("Fill", healthBarBG.transform);
            var fillRt = (RectTransform)healthBarFill.transform;
            fillRt.anchorMin = new Vector2(0, 0);
            fillRt.anchorMax = new Vector2(0, 1);
            fillRt.pivot = new Vector2(0, 0.5f);
            fillRt.offsetMin = Vector2.zero;
            fillRt.offsetMax = new Vector2(520, 0);
            var fillImg = AddImage(healthBarFill, new Color(0.3f, 0.8f, 0.3f));
            var healthLabelGO = NewUI("Label", healthBarBG.transform);
            Stretch(healthLabelGO);
            var healthLabel = AddText(healthLabelGO, "DRIVE HEALTH 100%", 24, Color.white, TextAlignmentOptions.Center);

            // HintToast
            var toastGO = NewUI("HintToast", canvasGO.transform);
            SetPos(toastGO, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -60), new Vector2(720, 96));
            AddImage(toastGO, new Color(0, 0, 0, 0.8f));
            var toastLabelGO = NewUI("Label", toastGO.transform);
            Stretch(toastLabelGO);
            var toastLabel = AddText(toastLabelGO, "", 32, new Color(0.4f, 1f, 0.4f), TextAlignmentOptions.Center);
            toastGO.SetActive(false);

            // Modal Layer
            var modalLayer = NewUI("ModalLayer", canvasGO.transform);
            Stretch(modalLayer);

            var winPanel = NewUI("WinPanel", modalLayer.transform);
            Stretch(winPanel);
            AddImage(winPanel, new Color(0, 0, 0, 0.92f));
            var winTitleGO = NewUI("Title", winPanel.transform);
            SetPos(winTitleGO, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -80), new Vector2(1400, 80));
            AddText(winTitleGO, "LAST WILL AND TESTAMENT", 64, new Color(0.4f, 1f, 0.4f), TextAlignmentOptions.Center);
            var winScrollGO = NewUI("Scroll", winPanel.transform);
            Fit(winScrollGO, new Vector2(0.1f, 0.2f), new Vector2(0.9f, 0.9f), Vector2.zero, new Vector2(0, -180));
            AddImage(winScrollGO, new Color(0.05f, 0.05f, 0.05f));
            var winScroll = winScrollGO.AddComponent<ScrollRect>();
            var winViewportGO = NewUI("Viewport", winScrollGO.transform);
            Fit(winViewportGO, Vector2.zero, Vector2.one, new Vector2(16, 16), new Vector2(-16, -16));
            AddImage(winViewportGO, new Color(0, 0, 0, 0.6f));
            winViewportGO.AddComponent<RectMask2D>();
            var winContentGO = NewUI("Content", winViewportGO.transform);
            var winContentRt = (RectTransform)winContentGO.transform;
            winContentRt.anchorMin = new Vector2(0, 1);
            winContentRt.anchorMax = new Vector2(1, 1);
            winContentRt.pivot = new Vector2(0.5f, 1);
            winContentRt.sizeDelta = new Vector2(0, 0);
            var winVLG = winContentGO.AddComponent<VerticalLayoutGroup>();
            winVLG.padding = new RectOffset(24, 24, 24, 24);
            winVLG.childControlWidth = true;
            winVLG.childControlHeight = true;
            winVLG.childForceExpandWidth = true;
            winVLG.childForceExpandHeight = false;
            var winCSF = winContentGO.AddComponent<ContentSizeFitter>();
            winCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var winWillTextGO = NewUI("Body", winContentGO.transform);
            var winWillText = AddText(winWillTextGO, "", 22, new Color(0.4f, 1f, 0.4f), TextAlignmentOptions.TopLeft);
            winScroll.viewport = (RectTransform)winViewportGO.transform;
            winScroll.content = winContentRt;
            winScroll.horizontal = false;
            var winButtonsGO = NewUI("Buttons", winPanel.transform);
            SetPos(winButtonsGO, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 80), new Vector2(900, 100));
            var winHLG = winButtonsGO.AddComponent<HorizontalLayoutGroup>();
            winHLG.spacing = 40; winHLG.childAlignment = TextAnchor.MiddleCenter;
            winHLG.childControlWidth = true; winHLG.childControlHeight = true; winHLG.childForceExpandWidth = true;
            var winRestartBtn = MakeButton("RESTART", winButtonsGO.transform);
            var winBootBtn = MakeButton("MAIN MENU", winButtonsGO.transform);
            winPanel.SetActive(false);

            var losePanel = NewUI("LosePanel", modalLayer.transform);
            Stretch(losePanel);
            AddImage(losePanel, new Color(0.1f, 0, 0, 0.95f));
            var loseTitleGO = NewUI("Title", losePanel.transform);
            SetPos(loseTitleGO, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(1400, 120));
            AddText(loseTitleGO, "DRIVE CORRUPTED", 96, new Color(1, 0.2f, 0.2f), TextAlignmentOptions.Center);
            var loseBodyGO = NewUI("Body", losePanel.transform);
            SetPos(loseBodyGO, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -40), new Vector2(1400, 120));
            AddText(loseBodyGO, "Grandpa's secrets are lost forever.\nYou are disinherited.", 36, Color.white, TextAlignmentOptions.Center);
            var loseBtn = MakeButton("TRY AGAIN", losePanel.transform);
            var loseBtnRt = (RectTransform)loseBtn.transform;
            loseBtnRt.anchorMin = new Vector2(0.5f, 0.5f);
            loseBtnRt.anchorMax = new Vector2(0.5f, 0.5f);
            loseBtnRt.sizeDelta = new Vector2(380, 100);
            loseBtnRt.anchoredPosition = new Vector2(0, -220);
            losePanel.SetActive(false);

            // Dead Man Switch timer banner (hidden until armed)
            var dmPanel = NewUI("DeadManPanel", modalLayer.transform);
            SetPos(dmPanel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(720, 80));
            AddImage(dmPanel, new Color(0.85f, 0.1f, 0.1f, 0.9f));
            var dmLabelGO = NewUI("Label", dmPanel.transform);
            Stretch(dmLabelGO);
            var dmLabel = AddText(dmLabelGO, "DEAD MAN SWITCH :: 1:00", 36, Color.white, TextAlignmentOptions.Center);
            dmLabel.fontStyle = FontStyles.Bold;
            dmPanel.SetActive(false);

            // Pause Menu — Win98 Start-menu style, pops from bottom-left above taskbar
            var pausePanel = NewUI("PausePanel", modalLayer.transform);
            Stretch(pausePanel);
            var pauseScrim = AddImage(pausePanel, new Color(0, 0, 0, 0.0f));
            pauseScrim.raycastTarget = true;
            var pauseScrimBtn = pausePanel.AddComponent<Button>();
            pauseScrimBtn.targetGraphic = pauseScrim;
            pauseScrimBtn.transition = Selectable.Transition.None;
            var pauseBoxGO = NewUI("Box", pausePanel.transform);
            var pauseBoxRt = (RectTransform)pauseBoxGO.transform;
            pauseBoxRt.anchorMin = new Vector2(0, 0);
            pauseBoxRt.anchorMax = new Vector2(0, 0);
            pauseBoxRt.pivot = new Vector2(0, 0);
            pauseBoxRt.sizeDelta = new Vector2(300, 180);
            pauseBoxRt.anchoredPosition = new Vector2(10, 72);
            AddImage(pauseBoxGO, Win98Gray);
            // Dark navy side-band (classic Win98 Start accent)
            var pauseBandGO = NewUI("Band", pauseBoxGO.transform);
            var pauseBandRt = (RectTransform)pauseBandGO.transform;
            pauseBandRt.anchorMin = new Vector2(0, 0);
            pauseBandRt.anchorMax = new Vector2(0, 1);
            pauseBandRt.pivot = new Vector2(0, 0.5f);
            pauseBandRt.sizeDelta = new Vector2(36, 0);
            pauseBandRt.anchoredPosition = Vector2.zero;
            AddImage(pauseBandGO, TitlebarNavy);
            // Buttons column
            var pauseBtnsGO = NewUI("Buttons", pauseBoxGO.transform);
            var pauseBtnsRt = (RectTransform)pauseBtnsGO.transform;
            pauseBtnsRt.anchorMin = new Vector2(0, 0);
            pauseBtnsRt.anchorMax = new Vector2(1, 1);
            pauseBtnsRt.offsetMin = new Vector2(44, 10);
            pauseBtnsRt.offsetMax = new Vector2(-8, -10);
            var pauseVLG = pauseBtnsGO.AddComponent<VerticalLayoutGroup>();
            pauseVLG.spacing = 6;
            pauseVLG.childAlignment = TextAnchor.UpperLeft;
            pauseVLG.childControlWidth = true; pauseVLG.childControlHeight = false;
            pauseVLG.childForceExpandWidth = true; pauseVLG.childForceExpandHeight = false;
            var pauseRestartBtn = MakeStartMenuButton("Restart", pauseBtnsGO.transform);
            var pauseMainMenuBtn = MakeStartMenuButton("Main Menu", pauseBtnsGO.transform);
            pausePanel.SetActive(false);

            // Managers
            var managers = new GameObject("Managers");
            var gm = managers.AddComponent<GameManager>();
            var ds = managers.AddComponent<DesktopManager>();
            var sp = managers.AddComponent<WindowSpawner>();
            var wa = managers.AddComponent<WillAssembler>();
            var ct = managers.AddComponent<CorruptionTimer>();
            var ao = managers.AddComponent<AudioOneShot>();
            var pm = managers.AddComponent<PauseMenu>();
            pm.startButton = startBtn;
            pm.panel = pausePanel;
            pm.scrimButton = pauseScrimBtn;
            pm.restartButton = pauseRestartBtn;
            pm.mainMenuButton = pauseMainMenuBtn;

            // Wire refs
            var desktopWindowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/DesktopWindow.prefab")?.GetComponent<WindowController>();
            var iconPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabDir}/DesktopIcon.prefab")?.GetComponent<IconBehavior>();
            var manifest = AssetDatabase.LoadAssetAtPath<DesktopManifest>($"{ContentDir}/DesktopManifest.asset");

            ds.iconPrefab = iconPrefab;
            ds.iconLayer = (RectTransform)iconLayer.transform;
            ds.manifest = manifest;
            ds.folderIcon = LoadIcon("directory_closed-4.png");
            ds.textIcon = LoadIcon("notepad_file-2.png");
            ds.imageIcon = LoadIcon("camera3-4.png");
            ds.passwordIcon = LoadIcon("executable-0.png");
            ds.videoIcon = LoadIcon("video_-2.png");
            ds.fragmentIcon = LoadIcon("notepad-2.png");
            ds.myComputerIcon = LoadIcon("computer-4.png");
            ds.recycleBinIcon = LoadIcon("recycle_bin_empty-4.png");
            ds.mailIcon = LoadIcon("mailbox_world-2.png");
            ds.encryptedIcon = LoadIcon("key_win-4.png");

            sp.windowPrefab = desktopWindowPrefab;
            sp.iconPrefabForFolders = iconPrefab;
            sp.textViewerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{ViewerDir}/TextViewer.prefab")?.GetComponent<TextViewer>();
            sp.imageViewerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{ViewerDir}/ImageViewer.prefab")?.GetComponent<ImageViewer>();
            var folderViewerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{ViewerDir}/FolderViewer.prefab")?.GetComponent<FolderViewer>();
            if (folderViewerPrefab != null) folderViewerPrefab.iconPrefab = iconPrefab;
            sp.folderViewerPrefab = folderViewerPrefab;
            sp.passwordGatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{ViewerDir}/PasswordGate.prefab")?.GetComponent<PasswordGate>();
            sp.videoViewerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{ViewerDir}/VideoViewer.prefab")?.GetComponent<VideoViewer>();
            sp.fragmentViewerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{ViewerDir}/WillFragmentViewer.prefab")?.GetComponent<WillFragmentViewer>();
            sp.windowLayer = (RectTransform)windowLayer.transform;

            gm.manifest = manifest;

            wa.fragmentCounter = fragCounter;
            wa.hintToast = toastGO;
            wa.hintToastText = toastLabel;
            wa.winPanel = winPanel;
            wa.winWillText = winWillText;
            wa.losePanel = losePanel;
            wa.winRestartButton = winRestartBtn;
            wa.winBootButton = winBootBtn;
            wa.loseRestartButton = loseBtn;
            wa.fullWillText = GetFullWillText();

            ct.fillBar = fillImg;
            ct.label = healthLabel;
            ct.deadManPanel = dmPanel;
            ct.deadManTimerLabel = dmLabel;

            // EventSystem + Camera
            MakeEventSystem();
            MakeMainCamera();

            EnsureScenesFolder();
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Desktop.unity");

            // Register in BuildSettings
            var bootScene = new EditorBuildSettingsScene("Assets/Scenes/Boot.unity", true);
            var desktopScene = new EditorBuildSettingsScene("Assets/Scenes/Desktop.unity", true);
            EditorBuildSettings.scenes = new[] { bootScene, desktopScene };

            Debug.Log("[WillExe] Desktop scene built.");
        }

        static Button MakeRadioOption(string label, Transform parent, out TMP_Text textRef)
        {
            var go = NewUI(label, parent);
            var bg = AddImage(go, new Color(0, 0, 0, 0f));
            bg.raycastTarget = true;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = bg;
            btn.transition = Selectable.Transition.None;
            var txtGO = NewUI("Label", go.transform);
            Stretch(txtGO);
            textRef = AddText(txtGO, $"\u25CB  {label}", 24, new Color(0.75f, 0.75f, 0.75f), TextAlignmentOptions.MidlineLeft);
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 38;
            return btn;
        }

        static Button MakeDifficultyPill(string title, string sub, Transform parent, out Image imageRef)
        {
            var go = NewUI(title, parent);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(0, 0);
            var img = AddImage(go, new Color(0.06f, 0.14f, 0.06f, 0.92f));
            imageRef = img;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var btnColors = btn.colors;
            btnColors.highlightedColor = new Color(0.10f, 0.24f, 0.10f, 1f);
            btnColors.pressedColor = new Color(0.04f, 0.18f, 0.04f, 1f);
            btn.colors = btnColors;
            var outline = go.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = new Color(0.35f, 0.75f, 0.4f, 1f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            var titleGO = NewUI("Title", go.transform);
            SetPos(titleGO, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -28), new Vector2(260, 46));
            var t = AddText(titleGO, $"[ {title} ]", 30, new Color(0.55f, 1f, 0.55f), TextAlignmentOptions.Center);
            t.fontStyle = FontStyles.Bold;
            var subGO = NewUI("Sub", go.transform);
            SetPos(subGO, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 28), new Vector2(260, 36));
            AddText(subGO, sub, 18, new Color(0.7f, 0.9f, 0.75f, 0.9f), TextAlignmentOptions.Center);
            return btn;
        }

        static Button MakeStartMenuButton(string label, Transform parent)
        {
            var go = NewUI(label, parent);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(0, 52);
            var img = AddImage(go, new Color(0.92f, 0.92f, 0.92f, 0f));
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.15f, 0.15f, 0.55f, 1f);
            colors.pressedColor = new Color(0.1f, 0.1f, 0.45f, 1f);
            btn.colors = colors;
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 52;
            var lblGO = NewUI("Label", go.transform);
            Fit(lblGO, Vector2.zero, Vector2.one, new Vector2(16, 0), new Vector2(-12, 0));
            var t = AddText(lblGO, label, 26, TextDark, TextAlignmentOptions.MidlineLeft);
            t.fontStyle = FontStyles.Bold;
            return btn;
        }

        static Button MakeButton(string label, Transform parent)
        {
            var go = NewUI(label, parent);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(360, 90);
            var img = AddImage(go, Win98Gray);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var lblGO = NewUI("Label", go.transform);
            Stretch(lblGO);
            AddText(lblGO, label, 32, TextDark, TextAlignmentOptions.Center);
            return btn;
        }

        static void MakeMainCamera()
        {
            var camGO = new GameObject("Main Camera", typeof(Camera));
            camGO.tag = "MainCamera";
            var cam = camGO.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.orthographic = true;
            cam.cullingMask = 0;
            cam.depth = -1;
        }

        static void MakeEventSystem()
        {
            var esGO = new GameObject("EventSystem", typeof(EventSystem));
            var inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputModuleType != null)
            {
                esGO.AddComponent(inputModuleType);
            }
            else
            {
                esGO.AddComponent<StandaloneInputModule>();
            }
        }

        static string GetFullWillText()
        {
            return
                "<b>LAST WILL AND TESTAMENT of [REDACTED]</b>\n\n" +
                "so. if you're reading this, i'm finally done. good for me.\n\n" +
                "the swiss vault. it's yours. ~$240M untraceable, 31kg of rough diamonds. combination's taped behind my high school yearbook, the one with the cracked spine. you'll know which.\n\n" +
                "the black book, original, also yours. forty years i sat on it. never used it once. that's the family record and i'd like you to try and beat it. if you can't, at least don't embarrass me.\n\n" +
                "the island, yes THAT one, is already rubble. i paid $5M up front to have it bulldozed and the ground salted. don't visit. don't drive past. i mean it.\n\n" +
                "biscuit's collar is in my desk, top-left drawer. he was a good dog. the best one.\n\n" +
                "keep posting the minion memes. the facebook, the dogecoin wallet, the chain emails, all of it. a man who looks like a harmless idiot online buys himself a lot of cover. don't throw that away.\n\n" +
                "the swap in '94 was the one clean thing i ever did. i don't regret any of the rest. that one i'm proud of.\n\n" +
                "run it better than i did. or burn it all down. either one, i'll understand.\n\n" +
                "- grandpa\n\n" +
                "<size=80%><color=#C06060>PS: they come for you in 48 hours. my enemies are yours now. welcome to the family business.</color></size>\n\n" +
                "<size=80%><color=#C06060>PPS: epstein didn't kill himself. i did. he was getting sloppy.</color></size>";
        }

        // ---------- Content (Manifest + IconDefs + WindowDefs) ----------
        [MenuItem("WillExe/4. Build Content")]
        public static void BuildContent()
        {
            if (!AssetDatabase.IsValidFolder($"{ContentDir}/Puzzles"))
                AssetDatabase.CreateFolder(ContentDir, "Puzzles");

            // Load generated hero sprites (silently no-op if PNG missing)
            var spYacht = LoadGeneratedSprite("yacht_1997.png");
            var spIslandOps = LoadGeneratedSprite("island_operations_97.png");
            var spFlight = LoadGeneratedSprite("flight_manifest_n908je.png");
            var spMinion = LoadGeneratedSprite("minion_live_laugh_load.png");
            var spDoge = LoadGeneratedSprite("doge_biscuit.png");
            var spRick = LoadGeneratedSprite("rickroll_still.png");
            var spBiscuit = LoadGeneratedSprite("biscuit_dog.png");
            var spNft = LoadGeneratedSprite("nft_receipt.png");
            var spArea51 = LoadGeneratedSprite("area51_aliens.png");

            // Win98 icon sprites (pixel-perfect point filter)
            var icFolder = LoadIcon("directory_closed-4.png");
            var icMyComputer = LoadIcon("computer-4.png");
            var icRecycle = LoadIcon("recycle_bin_empty-4.png");
            var icMail = LoadIcon("mailbox_world-2.png");
            var icTxt = LoadIcon("notepad_file-2.png");
            var icDoc = LoadIcon("notepad-2.png");
            var icImg = LoadIcon("camera3-4.png");
            var icExe = LoadIcon("executable-0.png");
            var icEnc = LoadIcon("key_win-4.png");
            var icVideo = LoadIcon("video_-2.png");
            var icDocWin = LoadIcon("file_windows-0.png");
            var icFolderOpen = LoadIcon("directory_open_file_mydocs-4.png");

            var rickFrames = LoadRickrollFrames();

            // --- PUZZLE 1: Boomer Facebook ---
            var biscuitPhoto = MkWindow("biscuit_jpg", "biscuit_goodboi.jpg", WindowType.Image,
                caption: "Me and Biscuit, 1987. best boy.");
            biscuitPhoto.imageSprite = spBiscuit;
            var minionMeme = MkWindow("minion_meme", "LIVE_LAUGH_LOAD.jpg", WindowType.Image,
                caption: "LIVE LAUGH LOAD (THE DISHWASHER)\n#blessed #wine-o-clock");
            minionMeme.imageSprite = spMinion;
            var chainEmail = MkWindow("prayer_chain", "FW_FW_PRAYER_CHAIN.eml", WindowType.Text,
                textBody:
                    "From: grandpa@aol.com\nTo: everyone-i-know\nSubject: FW: FW: FW: PRAYER CHAIN MUST READ\n\n" +
                    "FORWARD THIS TO 10 PEOPLE OR UR MOM DIES IN HER SLEEP TONIGHT TYPE AMEN IF U LOVE JESUS\n" +
                    "IGNORE IF U LOVE SATAN\n\n" +
                    "hope ur doing ok kid. the best friends are the four-legged kind. i miss the old days. - grandpa");
            var dogeRedHerring = MkWindow("doge_hint", "doge.jpg", WindowType.Image,
                caption: "such BISCUIT\nvery loyal\nwow\nmuch goodboi");
            dogeRedHerring.imageSprite = spDoge;
            var vineyardPhoto = MkWindow("vineyard_photo", "me_and_jeff_97.jpg", WindowType.Image,
                caption: "me + my best pal jeffrey, martha's vineyard 97. good times. good BOY.");
            vineyardPhoto.imageSprite = spYacht;

            var diaryTxt = MkWindow("diary_txt", "diary.txt (LOCKED)", WindowType.Password,
                passwordPrompt: "Enter best friend's name:",
                acceptedAnswers: new[] { "biscuit", "biscut", "biskit", "Biscuit" },
                unlocksOnSolve: "diary_unlocked",
                isWillFragment: true,
                fragmentText:
                    "FRAGMENT 1 of 4\n\n" +
                    "Dear diary,\n\n" +
                    "today biscuit died. also jeff called again about the island thing. he wants me to come back. i told him no.\n\n" +
                    "im getting too old for this. the operation is bigger than ever. i put my cut into that shibe currency the kids love.\n" +
                    "if anyone finds this diary, check the dogecoin wallet. year i hodl'd is the pin. - g");

            // --- PUZZLE 2: Crypto ---
            var hodlEmail = MkWindow("hodl_email", "Fwd_Fwd_HODL.eml", WindowType.Text,
                textBody:
                    "From: cryptodad69@aol.com\nSubject: FW: FW: FW: TO THE MOON BABY\n\n" +
                    "back when shibe was nothing. never sold. diamond hands baby. grandkids think i'm an idiot.\n" +
                    "they're going to be rich when i die. - grandpa");
            var nftReceipt = MkWindow("nft_receipt", "NFT_RECEIPT.jpg", WindowType.Image,
                caption: "DOGE.LEGACY #0069 :: wallet 0xDEAD...BEEF");
            nftReceipt.imageSprite = spNft;
            var dogeWallet = MkWindow("doge_wallet", "DogeWallet.exe", WindowType.Password,
                passwordPrompt: "Enter 4-digit wallet PIN:",
                acceptedAnswers: new[] { "2013" },
                unlocksOnSolve: "handler_folder_unlocked",
                isWillFragment: true,
                fragmentText:
                    "FRAGMENT 2 of 4\n\n" +
                    "the shibe money isn't the money. the shibe money is the cover. the real money is in the partition no one can see.\n" +
                    "if you're reading this, you've already bypassed layer 1. good. layer 2 is in SYSTEM32/HANDLER.\n" +
                    "it should be visible now. open it. meet the real me. - g");

            var cryptoFolder = MkWindow("crypto_folder", "CryptoBro", WindowType.Folder);
            var handlerFolder = MkWindow("handler_folder", "\\\\SYSTEM32\\\\HANDLER", WindowType.Folder);

            // --- PUZZLE 3: Handler reveal ---
            var islandOps = MkWindow("island_ops", "ISLAND_OPERATIONS_97.jpg", WindowType.Image,
                caption: "op briefing, martha's vineyard 97.\ni'm center. jeff is taking notes. jeff ALWAYS took notes.");
            islandOps.imageSprite = spIslandOps;
            var flightManifest = MkWindow("flight_manifest", "FLIGHT_MANIFEST_N908JE.pdf", WindowType.Image,
                caption: "private aviation log. heavily redacted.");
            flightManifest.imageSprite = spFlight;
            flightManifest.textBody = (
                    "N908JE :: LOLITA EXPRESS :: FLIGHT LOG\n\n" +
                    "ENTRY DATE: 1994-04-17\n" +
                    "ROLE     | NAME\n" +
                    "PILOT    | [GRANDPA]\n" +
                    "OPS LEAD | [GRANDPA]\n" +
                    "PAX #1   | J. EPSTEIN\n" +
                    "PAX #2   | [REDACTED]\n" +
                    "PAX #3   | [REDACTED]\n" +
                    "INFANT   | <ENTRY SWAPPED> :: see note\n\n" +
                    "NOTE (grandpa's handwriting): swapped the infant pax for a decoy. kid was 0 years old. won't happen to him again on my watch.");
            var blackBook = MkWindow("black_book", "THE_BLACK_BOOK.xlsx", WindowType.Text,
                textBody:
                    "THE BLACK BOOK :: mine. jeff wishes he had it.\n\n" +
                    "SENATOR A | affair :: 1991\n" +
                    "CEO B     | tax fraud :: 2002\n" +
                    "PRINCE C  | worse :: 199?\n" +
                    "JUDGE D   | affair + tax\n" +
                    "ACTOR E   | [sealed]\n" +
                    "... (3,847 more entries)");
            var epsteinTxt = MkWindow("epstein_txt", "EPSTEIN_DIDNT_KILL_HIMSELF.txt", WindowType.Text,
                textBody:
                    "Epstein didn't kill himself. Epstein didn't kill himself. Epstein didn't kill himself.\n" +
                    "Epstein didn't kill himself. Epstein didn't kill himself. Epstein didn't kill himself.\n" +
                    "Epstein didn't kill himself. Epstein didn't kill himself. Epstein didn't kill himself.\n\n" +
                    "(forwarded 47 times)");
            var diary2024 = MkWindow("diary_2024", "handler_diary_2024.txt", WindowType.Text,
                textBody:
                    "2024. 30 years since i swapped the manifest.\n" +
                    "burned my best operative to keep one file clean.\n" +
                    "if you're reading this, you made it. decrypt the file. - g");
            var area51 = MkWindow("area51", "AREA_51_ALIENS.bmp", WindowType.Image,
                caption: "AREA 51 PROOF!!! cover persona upkeep. boomers love this stuff.");
            area51.imageSprite = spArea51;
            var grandchildEnc = MkWindow("grandchild_enc", "protected.enc", WindowType.Password,
                passwordPrompt: "Enter the year of the swap:",
                acceptedAnswers: new[] { "1994" },
                unlocksOnSolve: "rickroll_unlocked",
                isWillFragment: true,
                fragmentText:
                    "FRAGMENT 3 of 4\n\n" +
                    "everything on this machine is mine. i built the machine the conspiracy theorists scream about.\n" +
                    "i was jeff's boss, not his friend. the one clean thing i ever did was keep you off the list.\n" +
                    "the rest is blood money, leverage, and enemies. run it better than i did. or burn it.\n\n" +
                    "one more thing. check the desktop. there's a file waiting for you.\n" +
                    "open it first. it contains my final instructions. i promise. - grandpa");

            // --- PUZZLE 4: Rickroll ---
            var rickrollVid = MkWindow("rickroll_vid", "final_message.mp4", WindowType.Video,
                caption: "",
                textBody:
                    "\n\n\n" +
                    "We're no strangers to love\n" +
                    "You know the rules and so do I (do I)\n" +
                    "A full commitment's what I'm thinking of\n" +
                    "You wouldn't get this from any other guy\n\n" +
                    "I just wanna tell you how I'm feeling\n" +
                    "Gotta make you understand\n\n" +
                    "Never gonna give you up\n" +
                    "Never gonna let you down\n" +
                    "Never gonna run around and desert you\n" +
                    "Never gonna make you cry\n" +
                    "Never gonna say goodbye\n" +
                    "Never gonna tell a lie and hurt you\n\n" +
                    "We've known each other for so long\n" +
                    "Your heart's been aching, but you're too shy to say it (say it)\n" +
                    "Inside, we both know what's been going on (going on)\n" +
                    "We know the game and we're gonna play it\n\n" +
                    "And if you ask me how I'm feeling\n" +
                    "Don't tell me you're too blind to see\n\n" +
                    "Never gonna give you up\n" +
                    "Never gonna let you down\n" +
                    "Never gonna run around and desert you\n" +
                    "Never gonna make you cry\n" +
                    "Never gonna say goodbye\n" +
                    "Never gonna tell a lie and hurt you\n\n" +
                    "Never gonna give, never gonna give (give you up)\n" +
                    "Never gonna give, never gonna give (give you up)\n\n" +
                    "- kid, my dead man switch is armed. trigger it with the obvious phrase.\n" +
                    "- SWITCH.exe in the Recycle Bin. last one. - g");
            rickrollVid.videoFrames = rickFrames;
            rickrollVid.videoFps = 15f;
            rickrollVid.imageSprite = spRick;
            var switchExe = MkWindow("switch_exe", "SWITCH.exe.locked", WindowType.Password,
                passwordPrompt: "[!] DEAD MAN SWITCH ARMED [!]\nthe bait, weaponized.",
                acceptedAnswers: new[] { "nevergiveyouup", "nevergiveup", "never gonna give you up", "never give you up" },
                unlocksOnSolve: "game_won",
                isWillFragment: true,
                fragmentText:
                    "FRAGMENT 4 of 4\n\n" +
                    "switch armed. contents deploying to FBI, NYT, WikiLeaks.\n" +
                    "effect: immediate. scope: total. enemies: activated.\n\n" +
                    "the will is assembled. your inheritance is live.\n" +
                    "check the desktop. run.");
            switchExe.forceRemainingSecondsOnOpen = true;
            switchExe.onOpenRemainingSeconds = 60f;

            // Icons
            var biscuitIcon = MkIcon("biscuit_icon", "biscuit_goodboi.jpg", biscuitPhoto);
            var minionIcon = MkIcon("minion_icon", "LIVE_LAUGH_LOAD.jpg", minionMeme);
            var prayerIcon = MkIcon("prayer_icon", "FW_FW_PRAYER.eml", chainEmail);
            var dogeIcon = MkIcon("doge_icon", "doge.jpg", dogeRedHerring);
            var vineyardIcon = MkIcon("vineyard_icon", "me_and_jeff_97.jpg", vineyardPhoto);
            var diaryIcon = MkIcon("diary_icon", "diary.txt", diaryTxt);

            var photosFolderWin = MkWindow("photos_folder", "Grandpa's Photos", WindowType.Folder);
            photosFolderWin.childIcons = new[] { biscuitIcon, minionIcon, dogeIcon, vineyardIcon };
            EditorUtility.SetDirty(photosFolderWin);

            var inboxFolderWin = MkWindow("inbox_folder", "Inbox", WindowType.Folder);
            inboxFolderWin.childIcons = new[] { prayerIcon, MkIcon("hodl_icon", "Fwd_HODL.eml", hodlEmail) };
            EditorUtility.SetDirty(inboxFolderWin);

            var hodlIconForCrypto = MkIcon("hodl_icon_crypto", "Fwd_HODL.eml", hodlEmail);
            var nftIcon = MkIcon("nft_icon", "NFT_RECEIPT.jpg", nftReceipt);
            var dogeWalletIcon = MkIcon("doge_wallet_icon", "DogeWallet.exe", dogeWallet);
            cryptoFolder.childIcons = new[] { hodlIconForCrypto, nftIcon, dogeWalletIcon };
            EditorUtility.SetDirty(cryptoFolder);

            var islandOpsIcon = MkIcon("island_ops_icon", "ISLAND_OPS_97.jpg", islandOps);
            var flightIcon = MkIcon("flight_icon", "N908JE.pdf", flightManifest);
            var blackBookIcon = MkIcon("blackbook_icon", "BLACK_BOOK.xlsx", blackBook);
            var epsteinIcon = MkIcon("epstein_icon", "EPSTEIN_DIDNT.txt", epsteinTxt);
            var diary2024Icon = MkIcon("diary2024_icon", "handler_diary.txt", diary2024);
            var area51Icon = MkIcon("area51_icon", "AREA_51.bmp", area51);
            var grandchildIcon = MkIcon("grandchild_icon", "protected.enc", grandchildEnc);
            handlerFolder.childIcons = new[] { islandOpsIcon, flightIcon, blackBookIcon, epsteinIcon, diary2024Icon, area51Icon, grandchildIcon };
            EditorUtility.SetDirty(handlerFolder);

            // Top level icons
            var myComputerWin = MkWindow("my_computer_folder", "My Computer", WindowType.Folder);
            myComputerWin.childIcons = new[] {
                MkIcon("photos_folder_icon", "Grandpa's Photos", photosFolderWin),
                MkIcon("inbox_folder_icon", "Inbox", inboxFolderWin),
                MkIcon("diary_locked_icon", "diary.txt", diaryTxt),
            };
            EditorUtility.SetDirty(myComputerWin);

            var myComputerIcon = MkIcon("mycomputer_desktop_icon", "My Computer", myComputerWin);
            myComputerIcon.desktopPosition = new Vector2(120, -120);

            var photosDeskIcon = MkIcon("photos_desktop_icon", "Grandpa's Photos", photosFolderWin);
            photosDeskIcon.desktopPosition = new Vector2(120, -280);

            var inboxDeskIcon = MkIcon("inbox_desktop_icon", "Inbox", inboxFolderWin);
            inboxDeskIcon.desktopPosition = new Vector2(120, -440);

            var cryptoDeskIcon = MkIcon("crypto_desktop_icon", "CryptoBro", cryptoFolder);
            cryptoDeskIcon.desktopPosition = new Vector2(120, -600);
            cryptoDeskIcon.requiredUnlockId = "diary_unlocked";

            var handlerDeskIcon = MkIcon("handler_desktop_icon", "\\\\SYSTEM32\\\\HANDLER", handlerFolder);
            handlerDeskIcon.desktopPosition = new Vector2(280, -120);
            handlerDeskIcon.requiredUnlockId = "handler_folder_unlocked";

            var rickrollDeskIcon = MkIcon("rickroll_desktop_icon", "final_message.mp4", rickrollVid);
            rickrollDeskIcon.desktopPosition = new Vector2(280, -280);
            rickrollDeskIcon.requiredUnlockId = "rickroll_unlocked";

            var recycleWin = MkWindow("recycle_bin_folder", "Recycle Bin", WindowType.Folder);
            recycleWin.childIcons = new[] { MkIcon("switch_icon", "SWITCH.exe.locked", switchExe) };
            EditorUtility.SetDirty(recycleWin);
            var recycleDeskIcon = MkIcon("recycle_desktop_icon", "Recycle Bin", recycleWin);
            recycleDeskIcon.desktopPosition = new Vector2(280, -440);
            recycleDeskIcon.requiredUnlockId = "rickroll_unlocked";

            // Manifest
            var manifest = AssetDatabase.LoadAssetAtPath<DesktopManifest>($"{ContentDir}/DesktopManifest.asset");
            if (manifest == null)
            {
                manifest = ScriptableObject.CreateInstance<DesktopManifest>();
                AssetDatabase.CreateAsset(manifest, $"{ContentDir}/DesktopManifest.asset");
            }
            manifest.topLevelIcons = new[] {
                myComputerIcon, photosDeskIcon, inboxDeskIcon,
                cryptoDeskIcon, handlerDeskIcon, rickrollDeskIcon, recycleDeskIcon
            };
            manifest.easyTimerSeconds = 0f;
            manifest.normalTimerSeconds = 900f;
            manifest.hardTimerSeconds = 480f;
            manifest.requiredFragments = 4;
            EditorUtility.SetDirty(manifest);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[WillExe] Content built.");
        }

        static WindowDef MkWindow(string id, string title, WindowType type,
            string textBody = "", string caption = "",
            string passwordPrompt = "Enter password:",
            string[] acceptedAnswers = null,
            string unlocksOnSolve = "",
            bool isWillFragment = false,
            string fragmentText = "")
        {
            string path = $"{ContentDir}/Puzzles/win_{id}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<WindowDef>(path);
            if (existing == null)
            {
                existing = ScriptableObject.CreateInstance<WindowDef>();
                AssetDatabase.CreateAsset(existing, path);
            }
            existing.id = id;
            existing.title = title;
            existing.type = type;
            existing.textBody = textBody;
            existing.caption = caption;
            existing.passwordPrompt = passwordPrompt;
            existing.acceptedAnswers = acceptedAnswers ?? new string[0];
            existing.unlocksOnSolve = unlocksOnSolve;
            existing.isWillFragment = isWillFragment;
            existing.fragmentText = fragmentText;
            existing.defaultSize = type == WindowType.Folder ? new Vector2(780, 520) :
                                    type == WindowType.Video ? new Vector2(900, 600) :
                                    new Vector2(700, 500);
            EditorUtility.SetDirty(existing);
            return existing;
        }

        static IconDef MkIcon(string id, string label, WindowDef window)
        {
            string path = $"{ContentDir}/Icons/icon_{id}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<IconDef>(path);
            if (existing == null)
            {
                existing = ScriptableObject.CreateInstance<IconDef>();
                AssetDatabase.CreateAsset(existing, path);
            }
            existing.id = id;
            existing.label = label;
            existing.window = window;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        [MenuItem("WillExe/BUILD ALL")]
        public static void BuildAll()
        {
            BuildContent();
            BuildPrefabs();
            BuildBoot();
            BuildDesktop();
            AssetDatabase.SaveAssets();
            // Leave Boot as the active scene so Play + builds start from the cover page
            EditorSceneManager.OpenScene("Assets/Scenes/Boot.unity", OpenSceneMode.Single);
            Debug.Log("[WillExe] Full build complete. Active scene: Boot.");
        }
    }
}
#endif
