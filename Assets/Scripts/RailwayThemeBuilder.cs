using UnityEngine;
using System.Collections.Generic;

public class RailwayThemeBuilder : MonoBehaviour
{
    public static RailwayThemeBuilder Instance;

    [Header("Colors & Visuals")]
    public Color ballastColor = new Color(0.18f, 0.18f, 0.20f);
    public Color railColor = new Color(0.70f, 0.72f, 0.75f);
    public Color tieColor = new Color(0.32f, 0.22f, 0.15f);
    public Color trainColor = new Color(0.12f, 0.14f, 0.20f);
    public Color trainAccentColor = new Color(0.85f, 0.15f, 0.15f);
    public Color barrierStripeColor = new Color(0.95f, 0.45f, 0.05f);
    public Color coinColor = new Color(1.0f, 0.85f, 0.1f);
    public Color gemColor = new Color(0.95f, 0.1f, 0.25f);
    public Color signalRedColor = new Color(1.0f, 0.1f, 0.1f);
    public Color signalGreenColor = new Color(0.1f, 1.0f, 0.2f);

    [Header("Audio References")]
    public AudioClip coinClip;
    public AudioClip gemClip;
    public AudioClip hitClip;

    private Material ballastMat;
    private Material railMat;
    private Material tieMat;
    private Material trainMat;
    private Material trainAccentMat;
    private Material barrierStripeMat;
    private Material whiteMat;
    private Material coinMat;
    private Material gemMat;
    private Material signalPostMat;
    private Material signalRedMat;
    private Material signalGreenMat;

    void Awake()
    {
        Instance = this;
        InitMaterials();
    }

    private void InitMaterials()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        if (shader == null) shader = Shader.Find("Diffuse");

        ballastMat = CreateMat(shader, ballastColor, 0.1f, 0.0f);
        railMat = CreateMat(shader, railColor, 0.8f, 0.9f);
        tieMat = CreateMat(shader, tieColor, 0.1f, 0.0f);
        trainMat = CreateMat(shader, trainColor, 0.6f, 0.3f);
        trainAccentMat = CreateMat(shader, trainAccentColor, 0.7f, 0.2f);
        barrierStripeMat = CreateMat(shader, barrierStripeColor, 0.3f, 0.1f);
        whiteMat = CreateMat(shader, Color.white, 0.3f, 0.1f);

        // Gold coin material with emission
        coinMat = CreateMat(shader, coinColor, 0.9f, 0.8f);
        coinMat.EnableKeyword("_EMISSION");
        coinMat.SetColor("_EmissionColor", coinColor * 0.4f);

        // Red gem material with glow
        gemMat = CreateMat(shader, gemColor, 0.95f, 0.4f);
        gemMat.EnableKeyword("_EMISSION");
        gemMat.SetColor("_EmissionColor", gemColor * 0.6f);

        signalPostMat = CreateMat(shader, new Color(0.2f, 0.2f, 0.2f), 0.5f, 0.3f);
        signalRedMat = CreateMat(shader, signalRedColor, 0.5f, 0.0f);
        signalRedMat.EnableKeyword("_EMISSION");
        signalRedMat.SetColor("_EmissionColor", signalRedColor * 1.5f);

        signalGreenMat = CreateMat(shader, signalGreenColor, 0.5f, 0.0f);
        signalGreenMat.EnableKeyword("_EMISSION");
        signalGreenMat.SetColor("_EmissionColor", signalGreenColor * 1.5f);
    }

    private Material CreateMat(Shader shader, Color col, float smoothness, float metallic)
    {
        Material mat = new Material(shader);
        mat.color = col;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", col);
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
        if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
        return mat;
    }

    public GameObject BuildRailwaySegment(Vector3 position, float length, int obstaclePattern)
    {
        if (ballastMat == null) InitMaterials();

        GameObject segment = new GameObject("RailwaySegment_" + Mathf.RoundToInt(position.z));
        segment.transform.position = position;

        // 1. Ballast Bed (ground)
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ballast_Ground";
        ground.transform.SetParent(segment.transform);
        ground.transform.localPosition = new Vector3(0f, -0.25f, length / 2f);
        ground.transform.localScale = new Vector3(14f, 0.5f, length);
        ground.GetComponent<Renderer>().material = ballastMat;

        // 2. Three Railway Tracks: Left (-3.5), Center (0.0), Right (+3.5)
        float[] laneXPositions = new float[] { -3.5f, 0f, 3.5f };
        for (int l = 0; l < laneXPositions.Length; l++)
        {
            float laneX = laneXPositions[l];

            // Steel Rails (two rails per lane spaced 1.4m apart)
            GameObject railL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railL.name = $"Rail_L_{l}";
            railL.transform.SetParent(segment.transform);
            railL.transform.localPosition = new Vector3(laneX - 0.7f, 0.12f, length / 2f);
            railL.transform.localScale = new Vector3(0.12f, 0.22f, length);
            railL.GetComponent<Renderer>().material = railMat;

            GameObject railR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railR.name = $"Rail_R_{l}";
            railR.transform.SetParent(segment.transform);
            railR.transform.localPosition = new Vector3(laneX + 0.7f, 0.12f, length / 2f);
            railR.transform.localScale = new Vector3(0.12f, 0.22f, length);
            railR.GetComponent<Renderer>().material = railMat;

            // Wooden Ties / Sleepers every 2m
            for (float z = 1f; z < length; z += 2.2f)
            {
                GameObject tie = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tie.name = "Tie";
                tie.transform.SetParent(segment.transform);
                tie.transform.localPosition = new Vector3(laneX, 0.05f, z);
                tie.transform.localScale = new Vector3(2.0f, 0.1f, 0.6f);
                tie.GetComponent<Renderer>().material = tieMat;
            }
        }

        // 3. Trackside Overhead Signal / Power Poles
        for (float z = 8f; z < length; z += 20f)
        {
            BuildCatenaryGantry(segment.transform, z);
        }

        // 4. Populate Obstacles & Collectibles based on pattern
        PopulateSegmentContents(segment.transform, length, obstaclePattern, laneXPositions);

        return segment;
    }

    private void BuildCatenaryGantry(Transform parent, float zPos)
    {
        // Left Pole
        GameObject poleL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        poleL.transform.SetParent(parent);
        poleL.transform.localPosition = new Vector3(-6.2f, 3.5f, zPos);
        poleL.transform.localScale = new Vector3(0.25f, 3.5f, 0.25f);
        poleL.GetComponent<Renderer>().material = signalPostMat;

        // Right Pole
        GameObject poleR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        poleR.transform.SetParent(parent);
        poleR.transform.localPosition = new Vector3(6.2f, 3.5f, zPos);
        poleR.transform.localScale = new Vector3(0.25f, 3.5f, 0.25f);
        poleR.GetComponent<Renderer>().material = signalPostMat;

        // Overhead Beam
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
        beam.transform.SetParent(parent);
        beam.transform.localPosition = new Vector3(0f, 7f, zPos);
        beam.transform.localScale = new Vector3(12.8f, 0.25f, 0.25f);
        beam.GetComponent<Renderer>().material = signalPostMat;
    }

    private void PopulateSegmentContents(Transform parent, float length, int pattern, float[] lanes)
    {
        // Patterns:
        // 0: Safe Segment (just coins & gem)
        // 1: Train on left or right, coins on other
        // 2: Barrier hurdle on center, coins on side
        // 3: Railway Signal on side, Train on opposite side
        // 4: Double barriers with gem in between

        switch (pattern)
        {
            case 0:
                SpawnCoinRow(parent, lanes[1], 10f, 35f, 4f);
                SpawnGem(parent, new Vector3(lanes[0], 1.2f, 25f));
                break;

            case 1:
                // Train on Left Lane
                BuildTrainObstacle(parent, new Vector3(lanes[0], 0f, 25f));
                // Coins on Center Lane
                SpawnCoinRow(parent, lanes[1], 12f, 38f, 4f);
                // Gem on Right Lane
                SpawnGem(parent, new Vector3(lanes[2], 1.2f, 20f));
                break;

            case 2:
                // Barrier hurdle on Center Lane (jumpable!)
                BuildBarrierObstacle(parent, new Vector3(lanes[1], 0f, 22f));
                // Train on Right Lane
                BuildTrainObstacle(parent, new Vector3(lanes[2], 0f, 30f));
                // Coins on Left Lane
                SpawnCoinRow(parent, lanes[0], 10f, 35f, 4f);
                // Gem above the barrier for jumping bonus
                SpawnGem(parent, new Vector3(lanes[1], 2.8f, 22f));
                break;

            case 3:
                // Signal Post obstacle on Right Lane
                BuildSignalObstacle(parent, new Vector3(lanes[2], 0f, 18f));
                // Train on Center Lane
                BuildTrainObstacle(parent, new Vector3(lanes[1], 0f, 32f));
                // Coins on Left Lane
                SpawnCoinRow(parent, lanes[0], 12f, 36f, 4f);
                break;

            case 4:
                // Barriers on Left and Right
                BuildBarrierObstacle(parent, new Vector3(lanes[0], 0f, 20f));
                BuildBarrierObstacle(parent, new Vector3(lanes[2], 0f, 28f));
                // Center safe coin run
                SpawnCoinRow(parent, lanes[1], 10f, 40f, 3.5f);
                SpawnGem(parent, new Vector3(lanes[1], 1.5f, 25f));
                break;
        }
    }

    public GameObject BuildTrainObstacle(Transform parent, Vector3 localPos)
    {
        GameObject train = new GameObject("TrainObstacle");
        train.tag = "Obstacle";
        train.transform.SetParent(parent);
        train.transform.localPosition = localPos;

        // Train Body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "TrainBody";
        body.tag = "Obstacle";
        body.transform.SetParent(train.transform);
        body.transform.localPosition = new Vector3(0f, 2.0f, 0f);
        body.transform.localScale = new Vector3(2.3f, 3.2f, 12.0f);
        body.GetComponent<Renderer>().material = trainMat;

        // Train Front Accent Stripe
        GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stripe.name = "AccentStripe";
        stripe.tag = "Obstacle";
        stripe.transform.SetParent(train.transform);
        stripe.transform.localPosition = new Vector3(0f, 1.4f, -5.95f);
        stripe.transform.localScale = new Vector3(2.32f, 0.45f, 0.2f);
        stripe.GetComponent<Renderer>().material = trainAccentMat;

        // Front Headlights
        GameObject lightL = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lightL.transform.SetParent(train.transform);
        lightL.transform.localPosition = new Vector3(-0.75f, 0.9f, -6.05f);
        lightL.transform.localScale = new Vector3(0.35f, 0.35f, 0.2f);
        lightL.GetComponent<Renderer>().material = coinMat; // Glowing yellow

        GameObject lightR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lightR.transform.SetParent(train.transform);
        lightR.transform.localPosition = new Vector3(0.75f, 0.9f, -6.05f);
        lightR.transform.localScale = new Vector3(0.35f, 0.35f, 0.2f);
        lightR.GetComponent<Renderer>().material = coinMat;

        // Windshield
        GameObject windshield = GameObject.CreatePrimitive(PrimitiveType.Cube);
        windshield.transform.SetParent(train.transform);
        windshield.transform.localPosition = new Vector3(0f, 2.6f, -5.95f);
        windshield.transform.localScale = new Vector3(1.8f, 0.9f, 0.15f);
        windshield.GetComponent<Renderer>().material = whiteMat;

        // Box collider covering the entire train
        BoxCollider col = train.AddComponent<BoxCollider>();
        col.size = new Vector3(2.3f, 3.2f, 12.0f);
        col.center = new Vector3(0f, 2.0f, 0f);

        // Attach TrainMover so the train moves oncoming towards the player!
        TrainMover mover = train.AddComponent<TrainMover>();
        mover.oncomingSpeed = 9.5f;

        return train;
    }

    public GameObject BuildBarrierObstacle(Transform parent, Vector3 localPos)
    {
        GameObject barrier = new GameObject("BarrierObstacle");
        barrier.tag = "Obstacle";
        barrier.transform.SetParent(parent);
        barrier.transform.localPosition = localPos;

        // Left and Right Posts
        GameObject postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        postL.tag = "Obstacle";
        postL.transform.SetParent(barrier.transform);
        postL.transform.localPosition = new Vector3(-1.05f, 0.65f, 0f);
        postL.transform.localScale = new Vector3(0.18f, 0.65f, 0.18f);
        postL.GetComponent<Renderer>().material = signalPostMat;

        GameObject postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        postR.tag = "Obstacle";
        postR.transform.SetParent(barrier.transform);
        postR.transform.localPosition = new Vector3(1.05f, 0.65f, 0f);
        postR.transform.localScale = new Vector3(0.18f, 0.65f, 0.18f);
        postR.GetComponent<Renderer>().material = signalPostMat;

        // Crossbar (Jumpable height: top is at ~1.3m)
        GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bar.tag = "Obstacle";
        bar.transform.SetParent(barrier.transform);
        bar.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        bar.transform.localScale = new Vector3(2.4f, 0.45f, 0.15f);
        bar.GetComponent<Renderer>().material = barrierStripeMat;

        // White stripes
        for (float sx = -0.8f; sx <= 0.8f; sx += 0.5f)
        {
            GameObject whiteStripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            whiteStripe.transform.SetParent(bar.transform);
            whiteStripe.transform.localPosition = new Vector3(sx / 2.4f, 0f, -0.05f);
            whiteStripe.transform.localScale = new Vector3(0.1f, 1.05f, 0.2f);
            whiteStripe.GetComponent<Renderer>().material = whiteMat;
        }

        BoxCollider col = barrier.AddComponent<BoxCollider>();
        col.size = new Vector3(2.4f, 1.3f, 0.4f);
        col.center = new Vector3(0f, 0.65f, 0f);

        return barrier;
    }

    public GameObject BuildSignalObstacle(Transform parent, Vector3 localPos)
    {
        GameObject signal = new GameObject("SignalObstacle");
        signal.tag = "Obstacle";
        signal.transform.SetParent(parent);
        signal.transform.localPosition = localPos;

        // Vertical Pole
        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.tag = "Obstacle";
        pole.transform.SetParent(signal.transform);
        pole.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        pole.transform.localScale = new Vector3(0.2f, 1.6f, 0.2f);
        pole.GetComponent<Renderer>().material = signalPostMat;

        // Signal Housing Box
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.tag = "Obstacle";
        box.transform.SetParent(signal.transform);
        box.transform.localPosition = new Vector3(0f, 2.7f, 0f);
        box.transform.localScale = new Vector3(0.6f, 1.2f, 0.4f);
        box.GetComponent<Renderer>().material = signalPostMat;

        // Red Light (top)
        GameObject redLight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        redLight.transform.SetParent(box.transform);
        redLight.transform.localPosition = new Vector3(0f, 0.25f, -0.52f);
        redLight.transform.localScale = new Vector3(0.5f, 0.35f, 0.2f);
        redLight.GetComponent<Renderer>().material = signalRedMat;

        // Green Light (bottom)
        GameObject greenLight = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        greenLight.transform.SetParent(box.transform);
        greenLight.transform.localPosition = new Vector3(0f, -0.25f, -0.52f);
        greenLight.transform.localScale = new Vector3(0.5f, 0.35f, 0.2f);
        greenLight.GetComponent<Renderer>().material = signalGreenMat;

        BoxCollider col = signal.AddComponent<BoxCollider>();
        col.size = new Vector3(0.8f, 3.4f, 0.8f);
        col.center = new Vector3(0f, 1.7f, 0f);

        return signal;
    }

    public void SpawnCoinRow(Transform parent, float x, float startZ, float endZ, float step)
    {
        for (float z = startZ; z <= endZ; z += step)
        {
            SpawnCoin(parent, new Vector3(x, 0.9f, z));
        }
    }

    public GameObject SpawnCoin(Transform parent, Vector3 localPos)
    {
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        coin.name = "Coin";
        coin.tag = "Coin";
        coin.transform.SetParent(parent);
        coin.transform.localPosition = localPos;
        coin.transform.localScale = new Vector3(0.85f, 0.08f, 0.85f);
        coin.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        coin.GetComponent<Renderer>().material = coinMat;

        // Add spinning script
        CollectableRotate rotate = coin.AddComponent<CollectableRotate>();
        rotate.rotateSpeed = 160f;
        rotate.rotationAxis = Vector3.forward;

        // Add collection script
        CollectCoin collect = coin.AddComponent<CollectCoin>();
        collect.collectSound = coinClip;

        // Set trigger
        Collider col = coin.GetComponent<Collider>();
        col.isTrigger = true;

        return coin;
    }

    public GameObject SpawnGem(Transform parent, Vector3 localPos)
    {
        // 3D Gem / Diamond (Octahedron style using rotated cube)
        GameObject gem = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gem.name = "Gem";
        gem.tag = "Gem";
        gem.transform.SetParent(parent);
        gem.transform.localPosition = localPos;
        gem.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        gem.transform.localRotation = Quaternion.Euler(45f, 45f, 45f);
        gem.GetComponent<Renderer>().material = gemMat;

        CollectableRotate rotate = gem.AddComponent<CollectableRotate>();
        rotate.rotateSpeed = 140f;
        rotate.rotationAxis = new Vector3(0.5f, 1f, 0.5f).normalized;

        GemCollect collect = gem.AddComponent<GemCollect>();
        collect.gemSound = gemClip;

        Collider col = gem.GetComponent<Collider>();
        col.isTrigger = true;

        return gem;
    }
}

