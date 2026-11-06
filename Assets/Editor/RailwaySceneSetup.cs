using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RailwaySceneSetup : MonoBehaviour
{
    private const string SCENES_PATH = "Assets/Scenes/";
    private const string PREFABS_PATH = "Assets/Prefabs/";
    private const string MATERIALS_PATH = "Assets/Materials/";

    [MenuItem("Railway Escape/Build All Scenes and Prefabs")]
    public static void BuildAllScenesAndPrefabs()
    {
        Debug.Log("[RailwaySceneSetup] Starting full build of Railway Escape scenes and prefabs...");

        EnsureDirectories();
        CreateMaterials();
        CreatePrefabs();

        CreateMainMenuScene();
        CreateStageSelectScene();
        CreateInformationScene();
        CreateGamePlayScene();

        UpdateBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[RailwaySceneSetup] Successfully built all 4 scenes and prefabs for Railway Escape!");
    }

    [MenuItem("Railway Escape/Build Windows Standalone")]
    public static void BuildWindowsStandalone()
    {
        string buildFolder = "D:/RailwayTrack/Build/";
        if (!System.IO.Directory.Exists(buildFolder)) System.IO.Directory.CreateDirectory(buildFolder);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new string[]
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/StageSelect.unity",
            "Assets/Scenes/Information.unity",
            "Assets/Scenes/GamePlay.unity"
        };
        buildPlayerOptions.locationPathName = buildFolder + "RailwayEscape.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log($"[RailwaySceneSetup] Standalone Build result: {report.summary.result}, errors: {report.summary.totalErrors}");
    }

    private static void EnsureDirectories()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
    }

    private static void CreateMaterials()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        SaveMaterial(shader, "Mat_Ballast", new Color(0.18f, 0.18f, 0.20f), 0.1f, 0f);
        SaveMaterial(shader, "Mat_SteelRail", new Color(0.72f, 0.74f, 0.78f), 0.85f, 0.9f);
        SaveMaterial(shader, "Mat_WoodTie", new Color(0.32f, 0.22f, 0.15f), 0.1f, 0f);
        SaveMaterial(shader, "Mat_TrainBody", new Color(0.12f, 0.14f, 0.20f), 0.6f, 0.3f);
        SaveMaterial(shader, "Mat_TrainAccent", new Color(0.85f, 0.15f, 0.15f), 0.7f, 0.2f);
        SaveMaterial(shader, "Mat_BarrierStripe", new Color(0.95f, 0.45f, 0.05f), 0.3f, 0.1f);
        SaveMaterial(shader, "Mat_White", Color.white, 0.3f, 0.1f);

        // Player Material (Athletic sporty cyan/teal in URP Lit - never pink!)
        Material playerMat = SaveMaterial(shader, "Player_Teal_Material", new Color(0.15f, 0.70f, 0.85f), 0.5f, 0.1f);

        Material goldMat = SaveMaterial(shader, "Mat_GoldCoin", new Color(1.0f, 0.85f, 0.1f), 0.9f, 0.8f);
        goldMat.EnableKeyword("_EMISSION");
        goldMat.SetColor("_EmissionColor", new Color(1.0f, 0.85f, 0.1f) * 0.5f);

        Material gemMat = SaveMaterial(shader, "Mat_RedGem", new Color(0.95f, 0.1f, 0.25f), 0.95f, 0.4f);
        gemMat.EnableKeyword("_EMISSION");
        gemMat.SetColor("_EmissionColor", new Color(0.95f, 0.1f, 0.25f) * 0.7f);

        SaveMaterial(shader, "Mat_SignalPost", new Color(0.2f, 0.2f, 0.2f), 0.5f, 0.3f);

        Material redLight = SaveMaterial(shader, "Mat_SignalRed", new Color(1f, 0.1f, 0.1f), 0.5f, 0f);
        redLight.EnableKeyword("_EMISSION");
        redLight.SetColor("_EmissionColor", new Color(1f, 0.1f, 0.1f) * 2f);

        Material greenLight = SaveMaterial(shader, "Mat_SignalGreen", new Color(0.1f, 1f, 0.2f), 0.5f, 0f);
        greenLight.EnableKeyword("_EMISSION");
        greenLight.SetColor("_EmissionColor", new Color(0.1f, 1f, 0.2f) * 2f);
    }

    private static Material SaveMaterial(Shader shader, string name, Color color, float smoothness, float metallic)
    {
        string path = MATERIALS_PATH + name + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.shader = shader;
        mat.color = color;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static Material GetMat(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Material>(MATERIALS_PATH + name + ".mat");
    }

    private static void CreatePrefabs()
    {
        // 1. Coin Prefab with new coin chime
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        coin.name = "Coin";
        coin.tag = "Coin";
        coin.transform.localScale = new Vector3(0.85f, 0.08f, 0.85f);
        coin.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        coin.GetComponent<Renderer>().material = GetMat("Mat_GoldCoin");
        coin.GetComponent<Collider>().isTrigger = true;
        CollectableRotate coinRot = coin.AddComponent<CollectableRotate>();
        coinRot.rotateSpeed = 160f;
        coinRot.rotationAxis = Vector3.forward;
        CollectCoin coinCol = coin.AddComponent<CollectCoin>();
        coinCol.collectSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/coin-pickup-chime.wav");
        PrefabUtility.SaveAsPrefabAsset(coin, PREFABS_PATH + "Coin.prefab");
        DestroyImmediate(coin);

        // 2. Gem Prefab
        GameObject gem = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gem.name = "Gem";
        gem.tag = "Gem";
        gem.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        gem.transform.rotation = Quaternion.Euler(45f, 45f, 45f);
        gem.GetComponent<Renderer>().material = GetMat("Mat_RedGem");
        gem.GetComponent<Collider>().isTrigger = true;
        CollectableRotate gemRot = gem.AddComponent<CollectableRotate>();
        gemRot.rotateSpeed = 140f;
        gemRot.rotationAxis = new Vector3(0.5f, 1f, 0.5f).normalized;
        GemCollect gemCol = gem.AddComponent<GemCollect>();
        gemCol.gemSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/gem-collect.wav");
        PrefabUtility.SaveAsPrefabAsset(gem, PREFABS_PATH + "Gem.prefab");
        DestroyImmediate(gem);

        // 3. Train Obstacle Prefab with Oncoming Movement towards player!
        GameObject train = new GameObject("TrainObstacle");
        train.tag = "Obstacle";
        GameObject trainBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trainBody.name = "TrainBody";
        trainBody.tag = "Obstacle";
        trainBody.transform.SetParent(train.transform);
        trainBody.transform.localPosition = new Vector3(0f, 2.0f, 0f);
        trainBody.transform.localScale = new Vector3(2.3f, 3.2f, 12.0f);
        trainBody.GetComponent<Renderer>().material = GetMat("Mat_TrainBody");

        GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stripe.name = "AccentStripe";
        stripe.tag = "Obstacle";
        stripe.transform.SetParent(train.transform);
        stripe.transform.localPosition = new Vector3(0f, 1.4f, -5.95f);
        stripe.transform.localScale = new Vector3(2.32f, 0.45f, 0.2f);
        stripe.GetComponent<Renderer>().material = GetMat("Mat_TrainAccent");

        GameObject lightL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lightL.transform.SetParent(train.transform);
        lightL.transform.localPosition = new Vector3(-0.75f, 0.9f, -6.05f);
        lightL.transform.localScale = new Vector3(0.35f, 0.35f, 0.2f);
        lightL.GetComponent<Renderer>().material = GetMat("Mat_GoldCoin");

        GameObject lightR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lightR.transform.SetParent(train.transform);
        lightR.transform.localPosition = new Vector3(0.75f, 0.9f, -6.05f);
        lightR.transform.localScale = new Vector3(0.35f, 0.35f, 0.2f);
        lightR.GetComponent<Renderer>().material = GetMat("Mat_GoldCoin");

        GameObject windshield = GameObject.CreatePrimitive(PrimitiveType.Cube);
        windshield.transform.SetParent(train.transform);
        windshield.transform.localPosition = new Vector3(0f, 2.6f, -5.95f);
        windshield.transform.localScale = new Vector3(1.8f, 0.9f, 0.15f);
        windshield.GetComponent<Renderer>().material = GetMat("Mat_White");

        BoxCollider trainCol = train.AddComponent<BoxCollider>();
        trainCol.size = new Vector3(2.3f, 3.2f, 12.0f);
        trainCol.center = new Vector3(0f, 2.0f, 0f);

        // Trains move oncoming towards player
        TrainMover trainMover = train.AddComponent<TrainMover>();
        trainMover.oncomingSpeed = 9.5f;

        PrefabUtility.SaveAsPrefabAsset(train, PREFABS_PATH + "TrainObstacle.prefab");
        DestroyImmediate(train);

        // 4. Barrier Obstacle Prefab
        GameObject barrier = new GameObject("BarrierObstacle");
        barrier.tag = "Obstacle";
        GameObject postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        postL.tag = "Obstacle";
        postL.transform.SetParent(barrier.transform);
        postL.transform.localPosition = new Vector3(-1.05f, 0.65f, 0f);
        postL.transform.localScale = new Vector3(0.18f, 0.65f, 0.18f);
        postL.GetComponent<Renderer>().material = GetMat("Mat_SignalPost");

        GameObject postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        postR.tag = "Obstacle";
        postR.transform.SetParent(barrier.transform);
        postR.transform.localPosition = new Vector3(1.05f, 0.65f, 0f);
        postR.transform.localScale = new Vector3(0.18f, 0.65f, 0.18f);
        postR.GetComponent<Renderer>().material = GetMat("Mat_SignalPost");

        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.tag = "Obstacle";
        bar.transform.SetParent(barrier.transform);
        bar.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        bar.transform.localScale = new Vector3(2.4f, 0.45f, 0.15f);
        bar.GetComponent<Renderer>().material = GetMat("Mat_BarrierStripe");

        for (float sx = -0.8f; sx <= 0.8f; sx += 0.5f)
        {
            GameObject whiteStripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            whiteStripe.transform.SetParent(bar.transform);
            whiteStripe.transform.localPosition = new Vector3(sx / 2.4f, 0f, -0.05f);
            whiteStripe.transform.localScale = new Vector3(0.1f, 1.05f, 0.2f);
            whiteStripe.GetComponent<Renderer>().material = GetMat("Mat_White");
        }

        BoxCollider barCol = barrier.AddComponent<BoxCollider>();
        barCol.size = new Vector3(2.4f, 1.3f, 0.4f);
        barCol.center = new Vector3(0f, 0.65f, 0f);
        PrefabUtility.SaveAsPrefabAsset(barrier, PREFABS_PATH + "BarrierObstacle.prefab");
        DestroyImmediate(barrier);

        // 5. Signal Obstacle Prefab
        GameObject signal = new GameObject("SignalObstacle");
        signal.tag = "Obstacle";
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.tag = "Obstacle";
        pole.transform.SetParent(signal.transform);
        pole.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        pole.transform.localScale = new Vector3(0.2f, 1.6f, 0.2f);
        pole.GetComponent<Renderer>().material = GetMat("Mat_SignalPost");

        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.tag = "Obstacle";
        box.transform.SetParent(signal.transform);
        box.transform.localPosition = new Vector3(0f, 2.7f, 0f);
        box.transform.localScale = new Vector3(0.6f, 1.2f, 0.4f);
        box.GetComponent<Renderer>().material = GetMat("Mat_SignalPost");

        GameObject rLight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rLight.transform.SetParent(box.transform);
        rLight.transform.localPosition = new Vector3(0f, 0.25f, -0.52f);
        rLight.transform.localScale = new Vector3(0.5f, 0.35f, 0.2f);
        rLight.GetComponent<Renderer>().material = GetMat("Mat_SignalRed");

        GameObject gLight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gLight.transform.SetParent(box.transform);
        gLight.transform.localPosition = new Vector3(0f, -0.25f, -0.52f);
        gLight.transform.localScale = new Vector3(0.5f, 0.35f, 0.2f);
        gLight.GetComponent<Renderer>().material = GetMat("Mat_SignalGreen");

        BoxCollider sigCol = signal.AddComponent<BoxCollider>();
        sigCol.size = new Vector3(0.8f, 3.4f, 0.8f);
        sigCol.center = new Vector3(0f, 1.7f, 0f);
        PrefabUtility.SaveAsPrefabAsset(signal, PREFABS_PATH + "SignalObstacle.prefab");
        DestroyImmediate(signal);

        // Also save Obstacle.prefab matching PPT slide
        GameObject genericObstacle = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "TrainObstacle.prefab"));
        genericObstacle.name = "Obstacle";
        PrefabUtility.SaveAsPrefabAsset(genericObstacle, PREFABS_PATH + "Obstacle.prefab");
        DestroyImmediate(genericObstacle);

        // 6. Segment Prefab (3-lane 50m railway track segment)
        CreateSegmentPrefab();

        // 7. Player Prefab
        CreatePlayerPrefab();
    }

    private static void CreateSegmentPrefab()
    {
        float length = 50f;
        GameObject segment = new GameObject("Segment");

        // Ballast Ground (wide 16m embankment)
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ballast_Ground";
        ground.transform.SetParent(segment.transform);
        ground.transform.localPosition = new Vector3(0f, -0.25f, length / 2f);
        ground.transform.localScale = new Vector3(16f, 0.5f, length);
        ground.GetComponent<Renderer>().material = GetMat("Mat_Ballast");

        // Side Curbs / Retaining walls
        GameObject curbL = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curbL.name = "SideCurb_L";
        curbL.transform.SetParent(segment.transform);
        curbL.transform.localPosition = new Vector3(-5.2f, 0.3f, length / 2f);
        curbL.transform.localScale = new Vector3(0.4f, 0.6f, length);
        curbL.GetComponent<Renderer>().material = GetMat("Mat_SignalPost");

        GameObject curbR = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curbR.name = "SideCurb_R";
        curbR.transform.SetParent(segment.transform);
        curbR.transform.localPosition = new Vector3(5.2f, 0.3f, length / 2f);
        curbR.transform.localScale = new Vector3(0.4f, 0.6f, length);
        curbR.GetComponent<Renderer>().material = GetMat("Mat_SignalPost");

        // 3 Railway Track lanes (-3.5, 0.0, +3.5)
        float[] laneXPositions = new float[] { -3.5f, 0f, 3.5f };
        for (int l = 0; l < laneXPositions.Length; l++)
        {
            float laneX = laneXPositions[l];
            // Left rail
            GameObject railL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railL.name = $"Rail_L_{l}";
            railL.transform.SetParent(segment.transform);
            railL.transform.localPosition = new Vector3(laneX - 0.7f, 0.12f, length / 2f);
            railL.transform.localScale = new Vector3(0.12f, 0.22f, length);
            railL.GetComponent<Renderer>().material = GetMat("Mat_SteelRail");

            // Right rail
            GameObject railR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railR.name = $"Rail_R_{l}";
            railR.transform.SetParent(segment.transform);
            railR.transform.localPosition = new Vector3(laneX + 0.7f, 0.12f, length / 2f);
            railR.transform.localScale = new Vector3(0.12f, 0.22f, length);
            railR.GetComponent<Renderer>().material = GetMat("Mat_SteelRail");

            // Wooden sleepers/ties
            for (float z = 1f; z < length; z += 2.2f)
            {
                GameObject tie = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tie.name = "Tie";
                tie.transform.SetParent(segment.transform);
                tie.transform.localPosition = new Vector3(laneX, 0.05f, z);
                tie.transform.localScale = new Vector3(2.0f, 0.1f, 0.6f);
                tie.GetComponent<Renderer>().material = GetMat("Mat_WoodTie");
            }
        }

        PrefabUtility.SaveAsPrefabAsset(segment, PREFABS_PATH + "Segment.prefab");
        DestroyImmediate(segment);
    }

    private static void CreatePlayerPrefab()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 0f, 0f);

        CharacterController cc = player.AddComponent<CharacterController>();
        cc.center = new Vector3(0f, 1.0f, 0f);
        cc.height = 2.0f;
        cc.radius = 0.45f;

        PlayerMovement movement = player.AddComponent<PlayerMovement>();
        movement.forwardSpeed = 11f;
        movement.lanePositions = new float[] { -3.5f, 0f, 3.5f };
        movement.currentLane = 1;
        movement.laneChangeSpeed = 18f;
        movement.leftLimit = -3.5f;
        movement.rightLimit = 3.5f;
        movement.jumpHeight = 2.4f;

        CollisionDetect collision = player.AddComponent<CollisionDetect>();
        collision.collisionSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/hit-obstacle.wav");

        // Character Visual Representation with full 3D mesh
        GameObject characterModelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/npc_casual_set_00/Prefabs/npc_csl_00_character_01f_01.prefab");
        GameObject characterInst = null;
        if (characterModelPrefab != null)
        {
            characterInst = (GameObject)PrefabUtility.InstantiatePrefab(characterModelPrefab, player.transform);
            characterInst.name = "RunnerCharacter";
            characterInst.transform.localPosition = Vector3.zero;
            characterInst.transform.localRotation = Quaternion.identity;
        }

        if (characterInst != null)
        {
            // Assign valid URP Lit material to all meshes so character is NEVER pink!
            Material tealMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Player_Teal_Material.mat");
            if (tealMat == null) tealMat = GetMat("Player_Teal_Material");
            if (tealMat == null) tealMat = GetMat("Mat_TrainAccent");

            Renderer[] rends = characterInst.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in rends)
            {
                Material[] shared = r.sharedMaterials;
                for (int i = 0; i < shared.Length; i++)
                {
                    shared[i] = tealMat;
                }
                r.sharedMaterials = shared;
            }

            // Attach Animator & PlayerRunController
            Animator anim = characterInst.GetComponent<Animator>();
            if (anim == null) anim = characterInst.AddComponent<Animator>();
            RuntimeAnimatorController rac = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/PlayerRunController.controller");
            anim.runtimeAnimatorController = rac;
            anim.applyRootMotion = false;
            movement.animator = anim;
        }

        PrefabUtility.SaveAsPrefabAsset(player, PREFABS_PATH + "Player.prefab");
        DestroyImmediate(player);
    }

    private static void CreateMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.09f, 0.14f);
        camObj.transform.position = new Vector3(0f, 3.5f, -8f);
        camObj.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
        camObj.AddComponent<AudioListener>();

        // Directional Light
        GameObject lightObj = new GameObject("Directional Light");
        Light dirLight = lightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = new Color(1f, 0.78f, 0.6f);
        dirLight.intensity = 1.1f;
        lightObj.transform.rotation = Quaternion.Euler(30f, -40f, 0f);

        // New Background Music
        GameObject musicObj = new GameObject("BackgroundMusic");
        AudioSource audioSource = musicObj.AddComponent<AudioSource>();
        audioSource.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/railway-runner-synth.wav");
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 0.5f;

        // Background Railway Diorama
        GameObject bgTrack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bgTrack.name = "MenuTrackGround";
        bgTrack.transform.position = new Vector3(0f, -0.25f, 10f);
        bgTrack.transform.localScale = new Vector3(16f, 0.5f, 40f);
        bgTrack.GetComponent<Renderer>().material = GetMat("Mat_Ballast");

        // Distant Glowing Moon / Sunset Sun
        GameObject moon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        moon.name = "SunsetSun";
        moon.transform.position = new Vector3(-8f, 10f, 35f);
        moon.transform.localScale = new Vector3(8f, 8f, 1f);
        moon.GetComponent<Renderer>().material = GetMat("Mat_GoldCoin");
        DestroyImmediate(moon.GetComponent<Collider>());

        // Static Showcase Train on right track
        GameObject trainPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "TrainObstacle.prefab");
        if (trainPrefab != null)
        {
            GameObject train = (GameObject)PrefabUtility.InstantiatePrefab(trainPrefab);
            train.transform.position = new Vector3(3.5f, 0f, 8f);
            // Disable movement in menu
            TrainMover tm = train.GetComponent<TrainMover>();
            if (tm != null) tm.isMoving = false;
        }

        // UI Canvas
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        MainMenuControl menuCtrl = canvasObj.AddComponent<MainMenuControl>();
        menuCtrl.stageSelectSceneName = "StageSelect";

        // Top-Right Score Badge (User request: moved single score to right corner, removed left 0 0)
        GameObject scoreBadge = CreateUIPanel(canvasObj.transform, "TopRightScoreBadge", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-110f, -40f), new Vector2(170f, 55f));
        scoreBadge.GetComponent<Image>().color = new Color(0.12f, 0.14f, 0.20f, 0.88f);
        scoreBadge.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);

        GameObject scoreLbl = CreateText(scoreBadge.transform, "ScoreLabel", "SCORE", 14, TextAlignmentOptions.Center, new Vector2(0f, 0.55f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        scoreLbl.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.85f, 0.2f);
        scoreLbl.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject scoreVal = CreateText(scoreBadge.transform, "ScoreValue", "0", 26, TextAlignmentOptions.Center, new Vector2(0f, 0f), new Vector2(1f, 0.65f), Vector2.zero, Vector2.zero);
        scoreVal.GetComponent<TextMeshProUGUI>().color = Color.white;
        scoreVal.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        menuCtrl.topScoreTMP = scoreVal.GetComponent<TextMeshProUGUI>();

        // Game Title (Cleaned up, no capstone project line as requested)
        GameObject titleText = CreateText(canvasObj.transform, "TitleText", "URBAN ESCAPE: RAILWAY ESCAPE", 42, TextAlignmentOptions.Center, new Vector2(0.5f, 0.76f), new Vector2(0.5f, 0.76f), Vector2.zero, new Vector2(850f, 80f));
        titleText.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.85f, 0.2f);
        titleText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Click to start prompt
        CreateText(canvasObj.transform, "PromptText", "CLICK TO START", 24, TextAlignmentOptions.Center, new Vector2(0.5f, 0.45f), new Vector2(0.5f, 0.45f), Vector2.zero, new Vector2(400f, 40f));

        // Start Game Button
        GameObject startBtnObj = CreateButton(canvasObj.transform, "StartGameButton", "Start Game", new Vector2(0.5f, 0.25f), new Vector2(260f, 65f), new Color(0.35f, 0.85f, 0.45f));
        Button startBtn = startBtnObj.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(startBtn.onClick, menuCtrl.StartGame);

        EditorSceneManager.SaveScene(scene, SCENES_PATH + "MainMenu.unity");
    }

    private static void CreateStageSelectScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.06f, 0.07f, 0.12f);
        camObj.transform.position = new Vector3(0f, 2.5f, -6f);
        camObj.transform.rotation = Quaternion.Euler(12f, 0f, 0f);
        camObj.AddComponent<AudioListener>();

        // Directional Light
        GameObject lightObj = new GameObject("Directional Light");
        Light dirLight = lightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = new Color(1f, 0.85f, 0.7f);
        dirLight.intensity = 1.2f;
        lightObj.transform.rotation = Quaternion.Euler(35f, -30f, 0f);

        // 3D Stage Pedestal Cube
        GameObject stageCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stageCube.name = "StagePreviewPedestal";
        stageCube.transform.position = new Vector3(0f, 0f, 0f);
        stageCube.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
        stageCube.GetComponent<Renderer>().material = GetMat("Mat_Ballast");

        // Mini Railway Rails on top of diorama
        GameObject rail1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rail1.transform.SetParent(stageCube.transform);
        rail1.transform.localPosition = new Vector3(-0.25f, 0.52f, 0f);
        rail1.transform.localScale = new Vector3(0.04f, 0.04f, 1f);
        rail1.GetComponent<Renderer>().material = GetMat("Mat_SteelRail");

        GameObject rail2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rail2.transform.SetParent(stageCube.transform);
        rail2.transform.localPosition = new Vector3(0.25f, 0.52f, 0f);
        rail2.transform.localScale = new Vector3(0.04f, 0.04f, 1f);
        rail2.GetComponent<Renderer>().material = GetMat("Mat_SteelRail");

        // Mini Signal on diorama
        GameObject miniSignal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        miniSignal.transform.SetParent(stageCube.transform);
        miniSignal.transform.localPosition = new Vector3(0.35f, 0.75f, 0.2f);
        miniSignal.transform.localScale = new Vector3(0.08f, 0.25f, 0.08f);
        miniSignal.GetComponent<Renderer>().material = GetMat("Mat_SignalPost");

        // Add gentle rotation to stage diorama
        CollectableRotate rot = stageCube.AddComponent<CollectableRotate>();
        rot.rotateSpeed = 25f;
        rot.rotationAxis = Vector3.up;

        // UI Canvas
        GameObject canvasObj = new GameObject("StageSelectCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        StageControls stageCtrl = canvasObj.AddComponent<StageControls>();
        stageCtrl.nextSceneName = "Information";
        stageCtrl.menuSceneName = "MainMenu";

        // Header Title (User requested removing hazards/theme line)
        GameObject header = CreateText(canvasObj.transform, "Header", "RAILWAY ESCAPE", 46, TextAlignmentOptions.Center, new Vector2(0.5f, 0.84f), new Vector2(0.5f, 0.84f), Vector2.zero, new Vector2(700f, 60f));
        header.GetComponent<TextMeshProUGUI>().color = Color.white;
        header.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Play Button
        GameObject playBtnObj = CreateButton(canvasObj.transform, "PlayButton", "PLAY", new Vector2(0.5f, 0.16f), new Vector2(240f, 65f), Color.white);
        playBtnObj.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        playBtnObj.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        Button playBtn = playBtnObj.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(playBtn.onClick, stageCtrl.PlaySelectedStage);

        // Back Button
        GameObject backBtnObj = CreateButton(canvasObj.transform, "BackButton", "BACK", new Vector2(0.12f, 0.16f), new Vector2(140f, 50f), new Color(0.3f, 0.3f, 0.35f));
        Button backBtn = backBtnObj.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(backBtn.onClick, stageCtrl.BackToMenu);

        EditorSceneManager.SaveScene(scene, SCENES_PATH + "StageSelect.unity");
    }

    private static void CreateInformationScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.09f, 0.14f);
        camObj.transform.position = new Vector3(0f, 2f, -5f);
        camObj.AddComponent<AudioListener>();

        // Directional Light
        GameObject lightObj = new GameObject("Directional Light");
        Light dirLight = lightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = new Color(0.85f, 0.85f, 1f);
        dirLight.intensity = 0.8f;

        // UI Canvas
        GameObject canvasObj = new GameObject("InformationCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        // Controls Display
        CreateKeyBox(canvasObj.transform, "KeyA", "A", "Move Left", new Vector2(0.38f, 0.65f));
        CreateText(canvasObj.transform, "OrText", "OR", 22, TextAlignmentOptions.Center, new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), Vector2.zero, new Vector2(60f, 40f));
        CreateKeyBox(canvasObj.transform, "KeyD", "D", "Move Right", new Vector2(0.62f, 0.65f));

        CreateKeyBox(canvasObj.transform, "KeySpace", "SPACEBAR", "Jump Over Barriers", new Vector2(0.5f, 0.38f), new Vector2(280f, 70f));

        CreateText(canvasObj.transform, "LoadingText", "LOADING . . .", 26, TextAlignmentOptions.Center, new Vector2(0.85f, 0.12f), new Vector2(0.85f, 0.12f), Vector2.zero, new Vector2(300f, 50f));

        GameObject fadeObj = CreateUIPanel(canvasObj.transform, "FadeOverlay", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image fadeImg = fadeObj.GetComponent<Image>();
        fadeImg.color = new Color(0f, 0f, 0f, 0f);
        fadeImg.raycastTarget = false;

        LoadToStage loadToStage = canvasObj.AddComponent<LoadToStage>();
        loadToStage.targetSceneName = "GamePlay";
        loadToStage.waitDuration = 3.2f;
        loadToStage.fadeOverlay = fadeImg;

        EditorSceneManager.SaveScene(scene, SCENES_PATH + "Information.unity");
    }

    private static void CreateGamePlayScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Directional Light
        GameObject lightObj = new GameObject("Directional Light");
        Light dirLight = lightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;
        dirLight.color = new Color(1.0f, 0.75f, 0.55f);
        dirLight.intensity = 1.25f;
        lightObj.transform.rotation = Quaternion.Euler(32f, -35f, 0f);

        // Ambient sky tint
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.3f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.12f, 0.12f, 0.20f);
        RenderSettings.fogDensity = 0.008f;

        // New Background Music
        GameObject musicObj = new GameObject("BackgroundMusic");
        AudioSource bMusic = musicObj.AddComponent<AudioSource>();
        bMusic.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/railway-runner-synth.wav");
        bMusic.loop = true;
        bMusic.playOnAwake = true;
        bMusic.volume = 0.5f;

        // RailwayThemeBuilder
        GameObject themeBuilderObj = new GameObject("RailwayThemeBuilder");
        RailwayThemeBuilder themeBuilder = themeBuilderObj.AddComponent<RailwayThemeBuilder>();
        themeBuilder.coinClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/coin-pickup-chime.wav");
        themeBuilder.gemClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/gem-collect.wav");
        themeBuilder.hitClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/hit-obstacle.wav");

        // Instantiate Player
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PREFABS_PATH + "Player.prefab");
        GameObject playerInst = null;
        if (playerPrefab != null)
        {
            playerInst = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            playerInst.transform.position = new Vector3(0f, 0f, 0f);
        }

        // Camera with CameraFollow
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.10f, 0.11f, 0.18f);
        camObj.AddComponent<AudioListener>();

        CameraFollow follow = camObj.AddComponent<CameraFollow>();
        if (playerInst != null) follow.target = playerInst.transform;
        follow.offset = new Vector3(0f, 4.5f, -6.5f);
        follow.pitchAngle = 18f;

        if (playerInst != null)
        {
            CollisionDetect colDetect = playerInst.GetComponent<CollisionDetect>();
            if (colDetect != null) colDetect.mainCamera = cam;
        }

        // Segment Generator
        GameObject generatorObj = new GameObject("SegmentGenerator");
        SegmentGenerator generator = generatorObj.AddComponent<SegmentGenerator>();
        if (playerInst != null) generator.playerTransform = playerInst.transform;
        generator.segmentLength = 50f;
        generator.initialSegments = 5;
        generator.spawnInterval = 3f;

        // UI Canvas (HUD)
        GameObject canvasObj = new GameObject("HUDCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        MasterLevelInfo levelInfo = canvasObj.AddComponent<MasterLevelInfo>();

        // Top-Left HUD (properly anchored with top-left pivot)
        GameObject hudPanel = CreateUIPanel(canvasObj.transform, "HUD_Panel", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(25f, -25f), new Vector2(180f, 135f));
        hudPanel.GetComponent<Image>().color = new Color(0.1f, 0.12f, 0.16f, 0.88f);
        hudPanel.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);

        CreateStatRow(hudPanel.transform, "HUD_Gem", -20f, new Color(0.95f, 0.15f, 0.25f), "GEM", out TextMeshProUGUI hudGemTMP);
        levelInfo.gemDisplayTMP = hudGemTMP;

        CreateStatRow(hudPanel.transform, "HUD_Coin", -60f, new Color(1f, 0.85f, 0.1f), "COIN", out TextMeshProUGUI hudCoinTMP);
        levelInfo.coinDisplayTMP = hudCoinTMP;

        CreateStatRow(hudPanel.transform, "HUD_Dist", -100f, new Color(0.2f, 0.85f, 0.35f), "DIST", out TextMeshProUGUI hudDistTMP);
        levelInfo.distanceDisplayTMP = hudDistTMP;

        // Top-Right Pause Button
        GameObject pauseBtnObj = CreateButton(canvasObj.transform, "PauseButton", "||", new Vector2(0.95f, 0.93f), new Vector2(55f, 55f), new Color(0.15f, 0.55f, 0.95f));
        pauseBtnObj.GetComponentInChildren<TextMeshProUGUI>().fontSize = 28;
        pauseBtnObj.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        PauseMenu pauseMenu = canvasObj.AddComponent<PauseMenu>();
        Button pauseBtn = pauseBtnObj.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(pauseBtn.onClick, pauseMenu.TogglePause);

        // Pause Menu Popup Panel
        GameObject pausePanel = CreateUIPanel(canvasObj.transform, "PausePanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(360f, 320f));
        pausePanel.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.95f);
        pausePanel.SetActive(false);
        pauseMenu.pausePanel = pausePanel;

        CreateText(pausePanel.transform, "PauseTitle", "GAME PAUSED", 32, TextAlignmentOptions.Center, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), Vector2.zero, new Vector2(300f, 40f));

        GameObject resumeBtnObj = CreateButton(pausePanel.transform, "ResumeBtn", "RESUME", new Vector2(0.5f, 0.62f), new Vector2(240f, 50f), new Color(0.25f, 0.75f, 0.35f));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(resumeBtnObj.GetComponent<Button>().onClick, pauseMenu.ResumeGame);

        GameObject restartBtnObj = CreateButton(pausePanel.transform, "RestartBtn", "RESTART", new Vector2(0.5f, 0.40f), new Vector2(240f, 50f), new Color(0.85f, 0.55f, 0.15f));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(restartBtnObj.GetComponent<Button>().onClick, pauseMenu.RestartLevel);

        GameObject menuBtnObj = CreateButton(pausePanel.transform, "MenuBtn", "MAIN MENU", new Vector2(0.5f, 0.18f), new Vector2(240f, 50f), new Color(0.35f, 0.35f, 0.4f));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(menuBtnObj.GetComponent<Button>().onClick, pauseMenu.GoToMainMenu);

        // Fade overlay
        GameObject fadeObj = CreateUIPanel(canvasObj.transform, "FadeOverlay", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image fadeImg = fadeObj.GetComponent<Image>();
        fadeImg.color = new Color(0f, 0f, 0f, 0f);
        fadeImg.raycastTarget = false;

        if (playerInst != null)
        {
            CollisionDetect colDetect = playerInst.GetComponent<CollisionDetect>();
            if (colDetect != null) colDetect.fadeOverlay = fadeImg;
        }

        EditorSceneManager.SaveScene(scene, SCENES_PATH + "GamePlay.unity");
    }

    private static void UpdateBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(SCENES_PATH + "MainMenu.unity", true),
            new EditorBuildSettingsScene(SCENES_PATH + "StageSelect.unity", true),
            new EditorBuildSettingsScene(SCENES_PATH + "Information.unity", true),
            new EditorBuildSettingsScene(SCENES_PATH + "GamePlay.unity", true),
        };
        EditorBuildSettings.scenes = scenes;
    }

    private static GameObject CreateUIPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        return panel;
    }

    private static void CreateStatRow(Transform parent, string rowName, float yOffset, Color iconColor, string iconLabel, out TextMeshProUGUI valTMP)
    {
        GameObject row = new GameObject(rowName);
        row.transform.SetParent(parent, false);
        RectTransform rt = row.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(10f, yOffset);
        rt.sizeDelta = new Vector2(-20f, 32f);

        // Icon Box
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(row.transform, false);
        RectTransform irt = icon.AddComponent<RectTransform>();
        irt.anchorMin = new Vector2(0f, 0.5f);
        irt.anchorMax = new Vector2(0f, 0.5f);
        irt.pivot = new Vector2(0f, 0.5f);
        irt.anchoredPosition = new Vector2(0f, 0f);
        irt.sizeDelta = new Vector2(28f, 28f);
        Image img = icon.AddComponent<Image>();
        img.color = iconColor;

        // Icon Label
        GameObject ilbl = new GameObject("IconLabel");
        ilbl.transform.SetParent(icon.transform, false);
        RectTransform ilblrt = ilbl.AddComponent<RectTransform>();
        ilblrt.anchorMin = Vector2.zero;
        ilblrt.anchorMax = Vector2.one;
        ilblrt.sizeDelta = Vector2.zero;
        TextMeshProUGUI ilblTMP = ilbl.AddComponent<TextMeshProUGUI>();
        ilblTMP.text = iconLabel[0].ToString();
        ilblTMP.fontSize = 18;
        ilblTMP.alignment = TextAlignmentOptions.Center;
        ilblTMP.color = Color.white;
        ilblTMP.fontStyle = FontStyles.Bold;

        // Value text
        GameObject val = new GameObject("Value");
        val.transform.SetParent(row.transform, false);
        RectTransform vrt = val.AddComponent<RectTransform>();
        vrt.anchorMin = new Vector2(0f, 0.5f);
        vrt.anchorMax = new Vector2(1f, 0.5f);
        vrt.pivot = new Vector2(0f, 0.5f);
        vrt.anchoredPosition = new Vector2(38f, 0f);
        vrt.sizeDelta = new Vector2(-40f, 30f);
        valTMP = val.AddComponent<TextMeshProUGUI>();
        valTMP.text = "0";
        valTMP.fontSize = 22;
        valTMP.alignment = TextAlignmentOptions.Left;
        valTMP.color = Color.white;
        valTMP.fontStyle = FontStyles.Bold;
    }

    private static GameObject CreateText(Transform parent, string name, string content, float fontSize, TextAlignmentOptions align, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        RectTransform rt = textObj.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.alignment = align;
        tmp.color = Color.white;
        return textObj;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Vector2 anchor, Vector2 size, Color bgColor)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;

        Image img = btnObj.AddComponent<Image>();
        img.color = bgColor;

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform trt = textObj.AddComponent<RectTransform>();
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 22;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.black;
        tmp.fontStyle = FontStyles.Bold;

        return btnObj;
    }

    private static void CreateKeyBox(Transform parent, string name, string keyName, string label, Vector2 anchor, Vector2 size = default)
    {
        if (size == default) size = new Vector2(100f, 90f);

        GameObject box = new GameObject(name);
        box.transform.SetParent(parent, false);
        RectTransform rt = box.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;

        Image img = box.AddComponent<Image>();
        img.color = new Color(0.2f, 0.22f, 0.28f, 0.9f);

        GameObject keyText = new GameObject("KeyText");
        keyText.transform.SetParent(box.transform, false);
        RectTransform krt = keyText.AddComponent<RectTransform>();
        krt.anchorMin = Vector2.zero;
        krt.anchorMax = Vector2.one;
        krt.sizeDelta = Vector2.zero;
        TextMeshProUGUI ktmp = keyText.AddComponent<TextMeshProUGUI>();
        ktmp.text = keyName;
        ktmp.fontSize = 28;
        ktmp.alignment = TextAlignmentOptions.Center;
        ktmp.color = Color.white;
        ktmp.fontStyle = FontStyles.Bold;

        GameObject actionText = new GameObject("ActionLabel");
        actionText.transform.SetParent(box.transform, false);
        RectTransform art = actionText.AddComponent<RectTransform>();
        art.anchorMin = new Vector2(0.5f, 1f);
        art.anchorMax = new Vector2(0.5f, 1f);
        art.anchoredPosition = new Vector2(0f, 25f);
        art.sizeDelta = new Vector2(250f, 30f);
        TextMeshProUGUI atmp = actionText.AddComponent<TextMeshProUGUI>();
        atmp.text = label;
        atmp.fontSize = 18;
        atmp.alignment = TextAlignmentOptions.Center;
        atmp.color = new Color(0.85f, 0.85f, 0.9f);
    }
}
