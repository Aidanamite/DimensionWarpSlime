using HarmonyLib;
using SRML;
using SRML.Console;
using SRML.SR;
using SRML.Utils;
using SRML.SR.Translation;
using SRML.Utils.Enum;
using SRML.SR.SaveSystem;
using SRML.SR.SaveSystem.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Reflection.Emit;
using MoSecretStyles;
using ExtendedItemInfo;
using AssetsLib;
using static AssetsLib.GameObjectUtils;
using static AssetsLib.MeshUtils;
using static AssetsLib.TextureUtils;
using Console = SRML.Console.Console;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace DimensionWarpSlime
{
    public class Main : ModEntryPoint
    {
        public static Main instance;
        internal static Assembly modAssembly = Assembly.GetExecutingAssembly();
        internal static string modName = $"{modAssembly.GetName().Name}";
        internal static string modDir = $"{Environment.CurrentDirectory}\\SRML\\Mods\\{modName}";
        internal static Dictionary<Identifiable.Id, Sprite> sprites = new Dictionary<Identifiable.Id, Sprite>();
        internal static Dictionary<Identifiable.Id, ZoneDirector.Zone> SpawnArea = new Dictionary<Identifiable.Id, ZoneDirector.Zone>();
        public static List<Identifiable.Id> ChronoDietIgnore = new List<Identifiable.Id>();
        public static List<Identifiable.Id> ChronoEatIgnore = new List<Identifiable.Id>();

        public override void PreLoad()
        {
            instance = this;
            ChronoDietIgnore.Add(Ids.CHRONO_SLIME);
            ChronoDietIgnore.Add(Ids.MATTER_SLIME);
            ChronoDietIgnore.Add(Ids.AUTOMATA_SLIME);
            HarmonyInstance.PatchAll();
            BasicSlimeRegistry(Ids.CHRONO_SLIME, Ids2.CHRONO_SLIME, LoadImage("slimeChrono.png", FilterMode.Bilinear, TextureWrapMode.Repeat).CreateSprite(), ZoneDirector.Zone.WILDS);
            BasicSlimeRegistry(Ids.MATTER_SLIME, Ids2.MATTER_SLIME, LoadImage("slimeMatter.png", FilterMode.Bilinear, TextureWrapMode.Repeat).CreateSprite(), ZoneDirector.Zone.REEF);
            BasicSlimeRegistry(Ids.AUTOMATA_SLIME, Ids2.AUTOMATA_SLIME, LoadImage("slimeAutomata.png", FilterMode.Bilinear, TextureWrapMode.Repeat).CreateSprite(), ZoneDirector.Zone.QUARRY);
            BasicSlimeRegistry(Ids.EMPATHY_SLIME, Ids2.EMPATHY_SLIME, LoadImage("slimeEmpathy.png", FilterMode.Bilinear, TextureWrapMode.Repeat).CreateSprite(), ZoneDirector.Zone.MOSS);
            BasicSlimeRegistry(Ids.REACTOR_SLIME, Ids2.REACTOR_SLIME, LoadImage("slimeReactor.png", FilterMode.Bilinear, TextureWrapMode.Repeat).CreateSprite(), ZoneDirector.Zone.DESERT);
        }

        public override void Load()
        {
            var pinkDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.PINK_SLIME);
            var tabbyDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.TABBY_SLIME);
            var radDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.RAD_SLIME);
            var luckyDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.LUCKY_SLIME);
            var puddleDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.PUDDLE_SLIME);
            var tarrDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.TARR_SLIME);
            var honeyDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.HONEY_SLIME);
            var phosDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.PHOSPHOR_SLIME);
            var glitchDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.GLITCH_SLIME);
            var crystalBallPre = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.CRYSTAL_BALL_TOY);

            //--------------------------------------------------------------------------------------------------------

            var def = Object.Instantiate(pinkDef);
            def.AppearancesDefault = new SlimeAppearance[] { Object.Instantiate(def.AppearancesDefault[0]) };
            def.CanLargofy = false;
            def.Diet = new SlimeDiet()
            {
                AdditionalFoods = new Identifiable.Id[0],
                Favorites = new Identifiable.Id[0],
                FavoriteProductionCount = 2,
                MajorFoodGroups = new SlimeEat.FoodGroup[0],
                Produces = new Identifiable.Id[0]
            };
            def.FavoriteToys = new Identifiable.Id[0];
            def.IdentifiableId = Ids.CHRONO_SLIME;
            def.IsLargo = false;
            def.Name = "Chrono Slime";
            def.name = "ChronoSlime";
            def.Diet.RefreshEatMap(GameContext.Instance.SlimeDefinitions, def);
            var a = def.AppearancesDefault[0];
            EditFace(a, (Color.red, Color.yellow, Color.red), (Color.white, Color.black, Color.yellow), "Chrono");
            a.Structures = new SlimeAppearanceStructure[] {
                new SlimeAppearanceStructure(a.Structures[0]),
                new SlimeAppearanceStructure(luckyDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[2]),
                new SlimeAppearanceStructure(radDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[1]),
                new SlimeAppearanceStructure(radDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[2])
                };
            a.ColorPalette.Ammo = new Color(0.5f,0.5f,0);
            a.ColorPalette.Top = new Color(0.1f, 0, 0.9f);
            a.ColorPalette.Middle = new Color(0.1f, 0, 0.9f);
            a.ColorPalette.Bottom = Color.white;

            var body = radDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[0].DefaultMaterials[0].Clone();
            body.name = "slimeChronoBase";
            a.Structures[0].DefaultMaterials[0] = body;
            body.SetColor("_TopColor", new Color(0.1f, 0, 0.9f));
            body.SetColor("_MiddleColor", new Color(0.2f, 0.2f, 0.9f));
            body.SetColor("_BottomColor", new Color(0.4f, 0.3f, 0.9f));
            a.Icon = sprites[Ids.CHRONO_SLIME];

            a.Structures[0].Element = a.Structures[0].Element.Clone("slimeChronoBody");
            var prefab = a.Structures[0].Element.Prefabs[0].gameObject.CreatePrefab();
            a.name = "slimeChrono_body";
            var skin = prefab.GetComponent<SkinnedMeshRenderer>();
            if (skin)
            {
                skin.sharedMesh = Object.Instantiate(skin.sharedMesh);
                skin.sharedMesh.name = "slimeChrono_body";
            }
            a.Structures[0].Element.Prefabs[0] = prefab.GetComponent<SlimeAppearanceObject>();

            Material clockBase = a.Structures[1].DefaultMaterials[0].Clone();
            clockBase.name = "slimeChrono_ClockBase";
            a.Structures[1].DefaultMaterials[0] = clockBase;
            clockBase.SetTexture("_StripeTexture", LoadImage("slimeChrono_stripe.png"));
            a.Structures[1].Element = CreateElement("slimeChronoClock", a.Structures[1].Element.Prefabs.ToArray());
            var clockPrefab = a.Structures[1].Element.Prefabs[0].CreatePrefab();
            clockPrefab.name = "slimeChronoClock";
            a.Structures[1].Element.Prefabs[0] = clockPrefab;
            var f = clockPrefab.GetComponent<MeshFilter>();
            var m = f.sharedMesh;
            m = CreateMesh(m.vertices, m.uv, m.triangles, x =>
                {
                    var d = new Vector2(x.x, x.z).magnitude;
                    if (x.y > 0 && ((x.y < 0.02f && d < 0.15f) || (d < 0.09f)))
                        x.y = 0;
                    else
                        x.y *= 1.5f;
                    return x;
                });
            f.sharedMesh = m;

            var childObj = new GameObject("clockFace", typeof(MeshRenderer), typeof(MeshFilter));
            childObj.transform.SetParent(clockPrefab.transform, false);
            childObj.transform.localPosition = Vector3.up * 0.02f;
            childObj.transform.localScale = Vector3.one * 1.1f;
            m = CreateMesh(m.vertices, m.triangles, m.uv, (x) => !(x.y == 0 && new Vector2(x.x, x.z).magnitude < 0.15f), (x) => x);
            childObj.GetComponent<MeshFilter>().sharedMesh = m;
            var clockBack = pinkDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[0].DefaultMaterials[0].Clone();
            clockBack.name = "slimeChrono_ClockBack";
            childObj.GetComponent<MeshRenderer>().sharedMaterial = clockBack;
            clockBack.SetColor("_TopColor", Color.white);
            clockBack.SetColor("_MiddleColor", Color.white);
            clockBack.SetColor("_BottomColor", Color.white);

            var handMesh = new Mesh();
            handMesh.vertices = new Vector3[]
            {
                new Vector3(0, 0, -0.01f),
                new Vector3(0, 0.015f, 0.03f),
                new Vector3(0.015f, 0, 0.03f),
                new Vector3(0, -0.015f, 0.03f),
                new Vector3(-0.015f, 0, 0.03f),
                new Vector3(0, 0, 0.1f)
            };
            handMesh.triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                5, 1, 4,
                5, 2, 1,
                5, 3, 2,
                5, 4, 3
            };
            handMesh.uv = new Vector2[]
            {
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero
            };
            handMesh.RecalculateBounds();
            handMesh.RecalculateNormals();
            handMesh.RecalculateTangents();

            var hand = new GameObject("clockHand (1)", typeof(MeshRenderer), typeof(MeshFilter));
            hand.transform.SetParent(childObj.transform, false);
            childObj.transform.localPosition = Vector3.up * 0.01f;
            childObj.transform.localScale = Vector3.one;
            hand.GetComponent<MeshFilter>().sharedMesh = handMesh;
            var clockHand = pinkDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[0].DefaultMaterials[0].Clone();
            clockHand.name = "slimeChrono_ClockHand";
            clockHand.SetColor("_TopColor", Color.black);
            clockHand.SetColor("_MiddleColor", Color.black);
            clockHand.SetColor("_BottomColor", Color.black);
            hand.GetComponent<MeshRenderer>().sharedMaterial = clockHand;

            hand = new GameObject("clockHand (2)", typeof(MeshRenderer), typeof(MeshFilter));
            hand.transform.SetParent(childObj.transform, false);
            childObj.transform.localPosition = Vector3.up * 0.01f;
            childObj.transform.localScale = Vector3.one * 0.9f;
            hand.GetComponent<MeshFilter>().sharedMesh = handMesh;
            hand.GetComponent<MeshRenderer>().sharedMaterial = clockHand;
            hand.transform.localScale *= 0.7f;

            childObj.AddComponent<ClockHandAnimator>();

            Material aura = a.Structures[2].DefaultMaterials[0].Clone();
            aura.name = "slimeChrono_Aura";
            a.Structures[2].DefaultMaterials[0] = aura;
            aura.SetColor("_MiddleColor", new Color(0.1f, 0, 0.9f));
            aura.SetColor("_EdgeColor", new Color(0, 0, 1, 0));
            aura.SetTexture("_Texture", LoadImage("slimeChrono_aura.png"));

            a.Structures[2].Element = new SlimeAppearanceElement()
            {
                Name = "slimeChronoAura",
                Prefabs = a.Structures[2].Element.Prefabs.ToArray()
            };
            var auraObj = a.Structures[2].Element.Prefabs[0].CreatePrefab();
            auraObj.name = "chrono_aura";
            a.Structures[2].Element.Prefabs[0] = auraObj;
            auraObj.transform.localScale /= 3;

            Material auraCore = a.Structures[3].DefaultMaterials[0].Clone();
            auraCore.name = "slimeChrono_AuraCore";
            a.Structures[3].DefaultMaterials[0] = auraCore;
            auraCore.SetColor("_Color", new Color(0.1f, 0, 0.9f, auraCore.GetColor("_Color").a));


            var slimePrefab = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.PINK_SLIME).CreatePrefab();
            slimePrefab.name = "slimeChrono";
            var id = slimePrefab.GetComponent<Identifiable>();
            id.id = Ids.CHRONO_SLIME;
            var app = slimePrefab.GetComponent<SlimeAppearanceApplicator>();
            app.SlimeDefinition = def;
            app.Appearance = a;
            var eat = slimePrefab.GetComponent<SlimeEat>();
            eat.slimeDefinition = def;
            var emo = slimePrefab.GetComponent<SlimeEmotions>();
            emo.initHunger = new SlimeEmotions.EmotionState(SlimeEmotions.Emotion.HUNGER, emo.initHunger.currVal, emo.initHunger.defVal, emo.initHunger.sensitivity / 2, emo.initHunger.recoveryPerGameHour / 2);
            emo.initAgitation = new SlimeEmotions.EmotionState(SlimeEmotions.Emotion.AGITATION, emo.initAgitation.currVal, emo.initAgitation.defVal, emo.initAgitation.sensitivity * 2, emo.initAgitation.recoveryPerGameHour);
            slimePrefab.AddComponent<TimeWarp>();
            LookupRegistry.RegisterIdentifiablePrefab(id);
            SlimeRegistry.RegisterSlimeDefinition(def);

            if (skin)
                GenerateBoneData(app, a, 0.2f);

            //--------------------------------------------------------------------------------------------------------

            def = Object.Instantiate(pinkDef);
            def.AppearancesDefault = new SlimeAppearance[] { Object.Instantiate(def.AppearancesDefault[0]) };
            def.CanLargofy = false;
            def.Diet = new SlimeDiet()
            {
                AdditionalFoods = def.Diet.AdditionalFoods,
                Favorites = new Identifiable.Id[0],
                FavoriteProductionCount = 2,
                MajorFoodGroups = def.Diet.MajorFoodGroups,
                Produces = new Identifiable.Id[] { Identifiable.Id.GOLD_ECHO }
            };
            def.FavoriteToys = new Identifiable.Id[0];
            def.IdentifiableId = Ids.MATTER_SLIME;
            def.IsLargo = false;
            def.Name = "Matter Slime";
            def.name = "MatterSlime";
            def.Diet.RefreshEatMap(GameContext.Instance.SlimeDefinitions, def);
            a = def.AppearancesDefault[0];
            EditFace(a, (new Color(0,0.125f,0.25f), Color.white, Color.black), (new Color(0, 0.125f, 0.25f), Color.black, Color.blue), "Matter");
            a.Structures = new SlimeAppearanceStructure[] {
                new SlimeAppearanceStructure(a.Structures[0])
                };
            a.ColorPalette.Ammo = new Color(0.5f, 0, 0.625f);
            a.ColorPalette.Top = new Color(0.5f, 0, 0.625f);
            a.ColorPalette.Middle = new Color(0.5f, 0, 0.625f);
            a.ColorPalette.Bottom = Color.white;

            body = crystalBallPre.GetComponent<Renderer>().material.Clone();
            body.name = "slimeMatter";
            a.Structures[0].DefaultMaterials[0] = body;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 2; j++)
                    body.SetColor($"_Color{i}{j}", body.GetColor($"_Color{i}{j}").Shift(0.5f, 0, 0.625f).Multiply(3,3,3));
            a.Icon = sprites[Ids.MATTER_SLIME];


            slimePrefab = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.PINK_SLIME).CreatePrefab();
            prefab = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.TARR_SLIME);
            slimePrefab.name = "slimeMatter";
            id = slimePrefab.GetComponent<Identifiable>();
            id.id = Ids.MATTER_SLIME;
            app = slimePrefab.GetComponent<SlimeAppearanceApplicator>();
            app.SlimeDefinition = def;
            app.Appearance = a;
            eat = slimePrefab.GetComponent<SlimeEat>();
            eat.slimeDefinition = def;
            var spawner = slimePrefab.AddComponent<IdleSpawner>();
            spawner.fx = prefab.GetComponent<TarrSpawnFX>().SpawnFX.CreatePrefab();
            spawner.fx.name = "slimeMatterIdleFx";
            foreach (var renderer in spawner.fx.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                if (renderer.mesh)
                {
                    if (renderer.sharedMaterial && renderer.sharedMaterial.name.Contains("slimeTarr"))
                        renderer.sharedMaterial = a.Structures[0].DefaultMaterials[0];
                    var main = renderer.GetComponent<ParticleSystem>().main;
                    main.startLifetimeMultiplier *= 5;
                    main.startSizeMultiplier /= 2;
                }
                else
                    Object.DestroyImmediate(renderer);
            }
            LookupRegistry.RegisterIdentifiablePrefab(id);
            SlimeRegistry.RegisterSlimeDefinition(def);

            //--------------------------------------------------------------------------------------------------------

            def = Object.Instantiate(pinkDef);
            def.AppearancesDefault = new SlimeAppearance[] { Object.Instantiate(def.AppearancesDefault[0]) };
            def.CanLargofy = false;
            def.Diet = new SlimeDiet()
            {
                AdditionalFoods = new Identifiable.Id[0],
                Favorites = new Identifiable.Id[0],
                FavoriteProductionCount = 2,
                MajorFoodGroups = new SlimeEat.FoodGroup[0],
                Produces = new Identifiable.Id[0]
            };
            def.FavoriteToys = new Identifiable.Id[0];
            def.IdentifiableId = Ids.AUTOMATA_SLIME;
            def.IsLargo = false;
            def.Name = "Automata Slime";
            def.name = "AutomataSlime";
            def.Diet.RefreshEatMap(GameContext.Instance.SlimeDefinitions, def);
            a = def.AppearancesDefault[0];
            EditFace(a, (Color.white, Color.red, Color.red), (Color.black, Color.black, Color.black), "Automata", (e,material) =>
            {
                if (material.HasProperty("_texcoord"))
                {
                    material.SetTextureScale("_texcoord", new Vector2(2f, 1f));
                    material.SetTextureOffset("_texcoord", new Vector2(-0.5f, 0f));
                }
            }, (e, material) =>
            {
                if (material.HasProperty("_texcoord"))
                {
                    material.SetTextureScale("_texcoord", new Vector2(2f, 1f));
                    material.SetTextureOffset("_texcoord", new Vector2(-0.5f, 0f));
                }
            });
            a.Structures = new SlimeAppearanceStructure[] {
                new SlimeAppearanceStructure(a.Structures[0])
                };
            a.ColorPalette.Ammo = new Color(0.7f, 0, 0);
            a.ColorPalette.Top = new Color(0.7f, 0, 0);
            a.ColorPalette.Middle = new Color(0.7f, 0, 0);
            a.ColorPalette.Bottom = Color.grey;

            a.Structures[0].Element = new SlimeAppearanceElement()
            {
                Name = "slimeAutomataBase",
                name = "slimeAutomataBase",
                Prefabs = a.Structures[0].Element.Prefabs.ToArray()
            };
            prefab = a.Structures[0].Element.Prefabs[0].gameObject.CreatePrefab();
            a.Structures[0].Element.Prefabs[0] = prefab.GetComponent<SlimeAppearanceObject>();
            skin = prefab.GetComponent<SkinnedMeshRenderer>();
            var nm = Object.Instantiate(skin.sharedMesh);
            skin.sharedMesh = nm;
            var uv = nm.uv;
            for (int i = 0; i < uv.Length; i++)
                uv[i] = uv[i] / new Vector2(2, 1) + new Vector2(0.25f, 0);
            nm.uv = uv;

            body = Resources.FindObjectsOfTypeAll<Material>().First((x) => x.name == "objSciFi03").Clone();
            body.name = "slimeAutomata";
            a.Structures[0].DefaultMaterials[0] = body;
            body.SetTexture("_ColorMask", LoadImage("slimeAutomata_mask.png"));
            body.SetTexture("_Normal", LoadImage("slimeAutomata_normal.png"));
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    body.SetColor($"_Color{i}{j}", body.GetColor($"_Color{i}{j}").Shift(1, 0, 0));
            a.Icon = sprites[Ids.AUTOMATA_SLIME];


            slimePrefab = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.PINK_SLIME).CreatePrefab();
            slimePrefab.name = "slimeAutomata";
            id = slimePrefab.GetComponent<Identifiable>();
            id.id = Ids.AUTOMATA_SLIME;
            app = slimePrefab.GetComponent<SlimeAppearanceApplicator>();
            app.SlimeDefinition = def;
            app.Appearance = a;
            eat = slimePrefab.GetComponent<SlimeEat>();
            eat.slimeDefinition = def;
            slimePrefab.AddComponent<DisableEmotions>().emotions = new SlimeEmotions.Emotion[] { SlimeEmotions.Emotion.AGITATION, SlimeEmotions.Emotion.FEAR };
            LookupRegistry.RegisterIdentifiablePrefab(id);
            SlimeRegistry.RegisterSlimeDefinition(def);

            //--------------------------------------------------------------------------------------------------------

            def = Object.Instantiate(pinkDef);
            def.AppearancesDefault = new SlimeAppearance[] { Object.Instantiate(def.AppearancesDefault[0]) };
            def.CanLargofy = false;
            def.Diet = new SlimeDiet()
            {
                AdditionalFoods = new Identifiable.Id[0],
                Favorites = new Identifiable.Id[0],
                FavoriteProductionCount = 2,
                MajorFoodGroups = new SlimeEat.FoodGroup[0],
                Produces = new Identifiable.Id[0]
            };
            def.FavoriteToys = new Identifiable.Id[0];
            def.IdentifiableId = Ids.EMPATHY_SLIME;
            def.IsLargo = false;
            def.Name = "Empathy Slime";
            def.name = "EmpathySlime";
            def.Diet.RefreshEatMap(GameContext.Instance.SlimeDefinitions, def);
            a = def.AppearancesDefault[0];
            EditFace(a, (new Color(0.5f,0,0), Color.black, Color.black), null, "Empathy");
            a.Structures = new SlimeAppearanceStructure[] {
                new SlimeAppearanceStructure(a.Structures[0]),
                new SlimeAppearanceStructure(a.Structures[0])
                };
            a.ColorPalette.Ammo = new Color(0, 0.8f, 0);
            a.ColorPalette.Top = new Color(0, 0.8f, 0);
            a.ColorPalette.Middle = Color.white;
            a.ColorPalette.Bottom = Color.white;

            a.Structures[0].Element = a.Structures[0].Element.Clone("slimeEmpathyBody");
            prefab = a.Structures[0].Element.Prefabs[0].gameObject.CreatePrefab();
            a.name = "slimeEmpathy_body";
            skin = prefab.GetComponent<SkinnedMeshRenderer>();
            skin.sharedMesh = Object.Instantiate(skin.sharedMesh);
            skin.sharedMesh.name = "slimeEmpathy_body";
            a.Structures[0].Element.Prefabs[0] = prefab.GetComponent<SlimeAppearanceObject>();

            var crest = a.Structures[0].DefaultMaterials[0].Clone();
            crest.name = "slimeEmpathyCrest";
            a.Structures[1].DefaultMaterials = new Material[] { crest };
            crest.SetColor("_TopColor", new Color(0.7f, 0.2f, 0.2f));
            crest.SetColor("_MiddleColor", new Color(0.6f, 0.2f, 0.2f));
            crest.SetColor("_BottomColor", new Color(0.3f, 0.1f, 0.1f));
            a.Structures[1].Element = CreateElement("slimeEmpathyCrest",a.Structures[1].Element.Prefabs[0].CreatePrefab());
            prefab = a.Structures[1].Element.Prefabs[0].gameObject;
            prefab.name = "slimeEmpathyCrest";
            a.Structures[1].SupportsFaces = false;
            var v = new List<Vector3>() { new Vector3(0.1f,0,0), new Vector3(-0.1f,0,0) };
            var t = new List<int>();
            var sv = new Vector3[]
            {
                new Vector3(0.09f,0.4f,0), new Vector3(0.06f,0.5f,0), new Vector3(0.02f,0.6f,0), new Vector3(0,0.65f,0), new Vector3(-0.02f,0.6f,0), new Vector3(-0.06f,0.5f,0), new Vector3(-0.09f,0.4f,0)
            };
            for (float i = -90; i <= 90; i += 180 / 10f)
            {
                var scale = (2 - Mathf.Abs(i / 90)) / 3;
                v.AddRange(sv.All((x) => true, (x) => x.Rotate(i, 0, 0) * scale));
                if (i == -90)
                    continue;
                t.AddRange(new int[]
                {
                    0, v.Count - sv.Length * 2, v.Count - sv.Length,
                    1, v.Count - 1, v.Count - 1 - sv.Length
                });
                for (int j = 1; j < sv.Length; j++)
                    t.AddRange(new int[]
                    {
                        v.Count - j, v.Count - j - 1, v.Count - j - 1 - sv.Length,
                        v.Count - j, v.Count - j - 1 - sv.Length, v.Count - j - sv.Length
                    });
            }
            prefab.GetComponent<SkinnedMeshRenderer>().sharedMesh = CreateMesh(v.ToArray(), new Vector2[v.Count], t.ToArray(),x => x.Rotate(15,0,0).Offset(0, 0.75f, 0.25f), x => x.Rotate(-15, 0, 0).Offset(0, 0.75f, -0.25f));

            body = tabbyDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[0].DefaultMaterials[0].Clone();
            body.name = "slimeEmpathy";
            a.Structures[0].DefaultMaterials[0] = body;
            body.SetTexture("_StripeTexture", LoadImage("slimeEmpathy_stripe.png"));
            body.SetColor("_TopColor", new Color(0.4f, 0.7f, 0.4f));
            body.SetColor("_MiddleColor", new Color(0.4f, 0.7f, 0.4f));
            body.SetColor("_BottomColor", Color.white);
            a.Icon = sprites[Ids.EMPATHY_SLIME];


            slimePrefab = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.PINK_SLIME).CreatePrefab();
            slimePrefab.name = "slimeEmpathy";
            id = slimePrefab.GetComponent<Identifiable>();
            id.id = Ids.EMPATHY_SLIME;
            app = slimePrefab.GetComponent<SlimeAppearanceApplicator>();
            app.SlimeDefinition = def;
            app.Appearance = a;
            eat = slimePrefab.GetComponent<SlimeEat>();
            eat.slimeDefinition = def;
            emo = slimePrefab.GetComponent<SlimeEmotions>();
            emo.initHunger = new SlimeEmotions.EmotionState(emo.initHunger.emotion, 0.5f, 0.5f, 0, 0);
            slimePrefab.AddComponent<Empathize>();
            slimePrefab.AddComponent<DisableEmotions>().emotions = new SlimeEmotions.Emotion[] { SlimeEmotions.Emotion.HUNGER };
            LookupRegistry.RegisterIdentifiablePrefab(id);
            SlimeRegistry.RegisterSlimeDefinition(def);

            GenerateBoneData(app, a, 0.5f);

            //--------------------------------------------------------------------------------------------------------

            def = Object.Instantiate(pinkDef);
            def.AppearancesDefault = new SlimeAppearance[] { Object.Instantiate(def.AppearancesDefault[0]) };
            def.CanLargofy = false;
            def.Diet = new SlimeDiet()
            {
                AdditionalFoods = new Identifiable.Id[0],
                Favorites = new Identifiable.Id[0],
                FavoriteProductionCount = 2,
                MajorFoodGroups = new SlimeEat.FoodGroup[0],
                Produces = new Identifiable.Id[0]
            };
            def.FavoriteToys = new Identifiable.Id[0];
            def.IdentifiableId = Ids.REACTOR_SLIME;
            def.IsLargo = false;
            def.Name = "Reactor Slime";
            def.name = "ReactorSlime";
            def.Diet.RefreshEatMap(GameContext.Instance.SlimeDefinitions, def);
            a = def.AppearancesDefault[0];
            EditFace(a, (new Color(1, 0.5f, 0), new Color(1, 0.5f, 0), new Color(1, 0.5f, 0)), (new Color(1, 0.5f, 0), new Color(0.5f, 0.35f, 0), new Color(0.3f, 0.15f, 0)), "Reactor");
            a.Structures = new SlimeAppearanceStructure[] {
                new SlimeAppearanceStructure(a.Structures[0]),
                new SlimeAppearanceStructure(glitchDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[1])
                };
            a.ColorPalette.Ammo = new Color(1, 0.5f, 0);
            a.ColorPalette.Top = new Color(1, 0.5f, 0);
            a.ColorPalette.Middle = new Color(0.5f, 0.25f, 0);
            a.ColorPalette.Bottom = Color.black;

            body = phosDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[0].DefaultMaterials[0].Clone();
            body.name = "slimeReactorBase";
            a.Structures[0].DefaultMaterials[0] = body;
            body.SetColor("_TopColor", new Color(0.5f, 0.25f, 0));
            body.SetColor("_MiddleColor", new Color(0.25f, 0.125f, 0));
            body.SetColor("_BottomColor", Color.black);
            body.SetColor("_GlowTop", new Color(1, 0.5f, 0));
            body.SetFloat("_GlowMin", 0.5f);
            a.Icon = sprites[Ids.REACTOR_SLIME];

            var orbCount = 8;
            var mat = Resources.FindObjectsOfTypeAll<Material>().First((x) => x.name == "Depth Water Ball").Clone();
            mat.name = "slimeReactorOrbTrail";
            mat.SetVector("_ColorMultiply", new Vector4(10, 5, 0, 10));
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(1, 0.5f, 0));
            tex.Apply();
            mat.SetTexture("_ColorRamp", tex);
            a.Structures[1].DefaultMaterials = new Material[orbCount];
            for (int i = 0; i < orbCount; i++)
                a.Structures[1].DefaultMaterials[i] = mat;
            var trailOrb = a.Structures[1].Element.Prefabs[0].gameObject.CreatePrefab();
            a.Structures[1].Element = new SlimeAppearanceElement()
            {
                name = "SlimeReactorOrbs",
                Name = "Slime Reactor Orbs",
                Prefabs = new SlimeAppearanceObject[orbCount]
            };
            a.Structures[1].ElementMaterials = new SlimeAppearanceMaterials[orbCount];
            for (int i = 0; i < orbCount; i++)
            {
                a.Structures[1].Element.Prefabs[i] = trailOrb.GetComponent<SlimeAppearanceObject>();
                a.Structures[1].ElementMaterials[i] = new SlimeAppearanceMaterials() { OverrideDefaults = false };
            }
            trailOrb.name = "slimeReactorOrb";
            mat = radDef.GetAppearanceForSet(SlimeAppearance.AppearanceSaveSet.CLASSIC).Structures[0].DefaultMaterials[0].Clone();
            mat.name = "slimeReactorOrb";
            mat.SetColor("_TopColor", new Color(1, 0.5f, 0));
            mat.SetColor("_MiddleColor", new Color(0.5f, 0.25f, 0));
            mat.SetColor("_BottomColor", Color.black);
            trailOrb.AddComponent<MeshRenderer>().sharedMaterial = mat;
            trailOrb.AddComponent<OrbitRandom>();
            nm = new Mesh();
            nm.vertices = new Vector3[]
            {
                new Vector3(0, -0.1f, 0),
                new Vector3(0, 0, 0.1f),
                new Vector3(0.1f, 0, 0),
                new Vector3(0, 0, -0.1f),
                new Vector3(-0.1f, 0, 0),
                new Vector3(0, 0.1f, 0)
            };
            nm.triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                5, 1, 4,
                5, 2, 1,
                5, 3, 2,
                5, 4, 3
            };
            nm.uv = new Vector2[]
            {
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero,
                Vector2.zero
            };
            nm.RecalculateBounds();
            nm.RecalculateNormals();
            nm.RecalculateTangents();
            trailOrb.AddComponent<MeshFilter>().mesh = nm;
            var trail = trailOrb.GetComponent<TrailRenderer>();
            trail.widthMultiplier *= 0.2f;
            trail.endWidth = 0;
            //trail.SetPositions(new Vector3[]);

            slimePrefab = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.PINK_SLIME).CreatePrefab();
            slimePrefab.name = "slimeReactor";
            id = slimePrefab.GetComponent<Identifiable>();
            id.id = Ids.REACTOR_SLIME;
            app = slimePrefab.GetComponent<SlimeAppearanceApplicator>();
            app.SlimeDefinition = def;
            app.Appearance = a;
            eat = slimePrefab.GetComponent<SlimeEat>();
            eat.slimeDefinition = def;
            var eaterPrefab = new GameObject("").CreatePrefab().AddComponent<EnergyCollector>();
            eaterPrefab.name = "slimeReactorEater";
            var col = eaterPrefab.gameObject.AddComponent<SphereCollider>();
            col.radius = 30;
            col.isTrigger = true;
            slimePrefab.AddComponent<SlimeEatEnergy>().eaterPrefab = eaterPrefab;
            LookupRegistry.RegisterIdentifiablePrefab(id);
            SlimeRegistry.RegisterSlimeDefinition(def);

            //if (SRModLoader.IsModPresent("mosecretstyles"))
                //MSS.DoTheThing();
        }

        public override void PostLoad()
        {
            if (SRModLoader.IsModPresent("extendediteminfo"))
                EII.DoTheThing();
        }

        public static void Log(string message) => instance.ConsoleInstance.Log(message);
        public static void LogError(string message) => instance.ConsoleInstance.LogError(message);
        public static void LogWarning(string message) => instance.ConsoleInstance.LogWarning(message);
        public static void LogSuccess(string message) => instance.ConsoleInstance.LogSuccess(message);

        static void BasicSlimeRegistry(Identifiable.Id ident, PediaDirector.Id pedia, Sprite sprit, ZoneDirector.Zone? spawnZone = null)
        {
            SlimeEat.FoodGroup.NONTARRGOLD_SLIMES.AddItem(ident);
            PediaRegistry.RegisterIdEntry(pedia, sprit);
            PediaRegistry.RegisterIdentifiableMapping(pedia, ident);
            PediaRegistry.SetPediaCategory(pedia, PediaRegistry.PediaCategory.SLIMES);
            AmmoRegistry.RegisterPlayerAmmo(PlayerState.AmmoMode.DEFAULT, ident);
            sprites[ident] = sprit;
            if (spawnZone != null)
                SpawnArea[ident] = spawnZone.Value;
        }

        internal static void EditFace(SlimeAppearance a, (Color,Color,Color)? eyes, (Color, Color, Color)? mouth, string name = "", Action<SlimeFace.SlimeExpression, Material> eyeAdditionalChanges = null, Action<SlimeFace.SlimeExpression, Material> mouthAdditionalChanges = null)
        {
            a.Face = Object.Instantiate(a.Face);
            var faces = a.Face.ExpressionFaces;
            var rep = new Dictionary<Material, Material>();
            for (int i = 0; i < faces.Length; i++)
            {
                if (eyes != null && faces[i].Eyes)
                {
                    if (!rep.TryGetValue(faces[i].Eyes, out var eye))
                    {
                        eye = faces[i].Eyes.Clone();
                        eye.name = eye.name.Replace("(Clone)", "").Replace(" (Instance)", "") + name;
                        if (eye.HasProperty("_EyeRed"))
                            eye.SetColor("_EyeRed", eyes.Value.Item1);
                        if (eye.HasProperty("_EyeGreen"))
                            eye.SetColor("_EyeGreen", eyes.Value.Item2);
                        if (eye.HasProperty("_EyeBlue"))
                            eye.SetColor("_EyeBlue", eyes.Value.Item3);
                        eyeAdditionalChanges?.Invoke(faces[i].SlimeExpression, eye);
                        rep.Add(faces[i].Eyes, eye);
                    }
                    faces[i].Eyes = eye;
                }
                if (mouth != null && faces[i].Mouth)
                {
                    if (!rep.TryGetValue(faces[i].Mouth, out var mout))
                    {
                        mout = faces[i].Mouth.Clone();
                        mout.name = mout.name.Replace("(Clone)", "").Replace(" (Instance)", "") + name;
                        if (mout.HasProperty("_MouthTop"))
                            mout.SetColor("_MouthTop", mouth.Value.Item1);
                        if (mout.HasProperty("_MouthMid"))
                            mout.SetColor("_MouthMid", mouth.Value.Item2);
                        if (mout.HasProperty("_MouthBot"))
                            mout.SetColor("_MouthBot", mouth.Value.Item3);
                        mouthAdditionalChanges?.Invoke(faces[i].SlimeExpression, mout);
                        rep.Add(faces[i].Mouth, mout);
                    }
                    faces[i].Mouth = mout;
                }
            }
            a.Face.OnEnable();
        }
    }

    static class EII
    {
        public static void DoTheThing()
        {
            var auto = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Ids.AUTOMATA_SLIME);
            var chrono = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Ids.CHRONO_SLIME);
            var matter = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Ids.MATTER_SLIME);
            Func<IEnumerable<Identifiable.Id>> autoEat = delegate
            {
                var e = Identifiable.CHICK_CLASS.ToList();
                e.AddRange(Identifiable.MEAT_CLASS);
                e.Add(Identifiable.Id.PLAYER);
                return e;
            };
            foreach (var def in GameContext.Instance.SlimeDefinitions.slimeDefinitionsByIdentifiable.Values)
            {
                if (def.Matches(Ids.AUTOMATA_SLIME))
                {
                    ExtendedItemInfo.Main.CanEatOverrides[def.Diet] = autoEat;
                    ExtendedItemInfo.Main.CanProduceOverrides[def.Diet] = delegate
                    {
                        var e = new List<Identifiable.Id>();
                        foreach (var i in autoEat())
                            if (GameContext.Instance.LookupDirector.identifiablePrefabDict.TryGetValue(i, out var g) && g)
                            {
                                var t1 = g.GetComponent<TransformAfterTime>();
                                var t2 = g.GetComponent<TransformChanceOnReproduce>();
                                if (t1)
                                    e.AddRangeUnique(t1.options.All(x => x?.targetPrefab?.GetComponent<Identifiable>(), x => x.targetPrefab.GetComponent<Identifiable>().id));
                                if (t2?.targetPrefab?.GetComponent<Identifiable>())
                                    e.AddUnique(t2.targetPrefab.GetComponent<Identifiable>().id);
                            }
                        return e;
                    };
                }
                if (def.Matches(Ids.CHRONO_SLIME))
                {
                    ExtendedItemInfo.Main.CanEatOverrides[def.Diet] = delegate
                    {
                        var added = chrono.Diet.EatMap.All((x) => x.producesId != Identifiable.Id.NONE, (x) => x.producesId, true);
                        foreach (var d in GameContext.Instance.SlimeDefinitions.slimeDefinitionsByIdentifiable.Values)
                            if (!Main.ChronoDietIgnore.Exists(x => d.Matches(x)) && d.Diet != null && d.Diet.EatMap != null)
                                foreach (var em in d.Diet.EatMap)
                                    if (!Main.ChronoEatIgnore.Contains(em.eats) && em.producesId != Identifiable.Id.NONE)
                                        added.AddUnique(em.producesId);
                        return added;
                    };
                    ExtendedItemInfo.Main.CanProduceOverrides[def.Diet] = delegate
                    {
                        var maps = new List<Identifiable.Id>();
                        foreach (var em in chrono.Diet.EatMap)
                            foreach (var d in GameContext.Instance.SlimeDefinitions.slimeDefinitionsByIdentifiable.Values)
                                if (!Main.ChronoDietIgnore.Exists(x => d.Matches(x)) && d.Diet != null)
                                    foreach (var mapValues in d.Diet.EatMap)
                                        if (mapValues.producesId == em.eats && mapValues.NumToProduce() > 0)
                                            maps.AddUnique(mapValues.eats);
                        return maps;
                    };
                }
                if (def.Matches(Ids.MATTER_SLIME))
                    ExtendedItemInfo.Main.CanProduceOverrides[matter.Diet] = delegate { return null; };
            }
        }
    }
    /*
    static class MSS
    {
        public static void DoTheThing()
        {
            ModSecretStyle.onSecretStylesInitialization += delegate
            {
                var pinkDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.PINK_SLIME);
                var pinkMat = pinkDef.AppearancesDefault[0].Structures[0].DefaultMaterials[0];
                var tabbyDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.TABBY_SLIME);
                var tabbyMat = tabbyDef.AppearancesDefault[0].Structures[0].DefaultMaterials[0];
                var radDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.RAD_SLIME);
                var luckyDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.LUCKY_SLIME);
                var luckyCoinMat = luckyDef.AppearancesDefault[0].Structures[2].DefaultMaterials[0];
                var puddleDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.PUDDLE_SLIME);
                var tarrDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.TARR_SLIME);
                var honeyDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.HONEY_SLIME);
                var honeyMat = honeyDef.AppearancesDefault[0].Structures[0].DefaultMaterials[0];
                var phosDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.PHOSPHOR_SLIME);
                var glitchDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.GLITCH_SLIME);
                var goldDef = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(Identifiable.Id.GOLD_SLIME);
                var goldMat = goldDef.AppearancesDefault[0].Structures[0].DefaultMaterials[0];
                var crystalBallPre = GameContext.Instance.LookupDirector.GetPrefab(Identifiable.Id.CRYSTAL_BALL_TOY);


                var ss = new ModSecretStyle(Ids.EMPATHY_SLIME);
                ss.SecretStyle.NameXlateKey = "l.secret_style_empathy";
                ss.SecretStyle.Structures = new[]
                {
                    new SlimeAppearanceStructure(pinkDef.AppearancesDefault[0].Structures[0]),
                    new SlimeAppearanceStructure(ss.SecretStyle.Structures[0])
                };

                ss.SecretStyle.Structures[0].DefaultMaterials = new[] { honeyMat.Clone() };
                ss.SecretStyle.Structures[0].DefaultMaterials[0].name = "slimeEmpathyExoticBase";
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_TopColor", new Color(0.1f,0.8f,0.1f));
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_MiddleColor", new Color(0.1f, 0.6f, 0.1f));
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_BottomColor", new Color(0, 0.3f, 0));
                ss.SecretStyle.Structures[0].Element = ss.SecretStyle.Structures[0].Element.Clone("slimeEmpathyExoticBody");
                var a = ss.SecretStyle.Structures[0].Element.Prefabs[0].CreatePrefab();
                a.name = "slimeEmpathyExotic_body";
                var s = a.GetComponent<SkinnedMeshRenderer>();
                s.sharedMesh = Object.Instantiate(s.sharedMesh);
                s.sharedMesh.name = "slimeEmpathyExotic_body";
                ss.SecretStyle.Structures[0].Element.Prefabs[0] = a;

                ss.SecretStyle.Structures[1].DefaultMaterials = new[] { goldMat };
                var step = 10;
                var step2 = 10;
                var radius = 0.1f;
                var v = new List<Vector3>() { Vector3.zero.Rotate(-180, 0, 0, 0, 0, -radius * 2).Offset(0, 0.5f, 0) };
                var u = new List<Vector2>() { new Vector2(0.5f, 1) };
                var t = new List<int>();
                for (int j = 0; j < step2; j++)
                    t.AddRange(new int[] { 0, j + 1, (j + 1) % step2 + 1 });
                for (int i = step - 1; i >= 0; i--)
                {
                    var y = 1f / step * i;
                    var x = (1-y) * radius;
                    for (int j = 0; j < step2; j++)
                    {
                        v.Add(new Vector3(x, 0, 0).Rotate(0, 360f / step2 * j, 0).Rotate(-180 * y, 0, 0, 0, 0, -radius * 2).Offset(0, y * 0.5f, 0));
                        u.Add(new Vector2(j / step2, y));
                    }
                    if (i < step - 1)
                        for (int j = 0; j < step2; j++)
                            t.AddRange(new int[]
                                {
                                    v.Count - (step2 * 2) + j, v.Count - step2 + j, v.Count - step2 + (j + 1) % step2,
                                    v.Count - (step2 * 2) + j, v.Count - step2 + (j + 1) % step2, v.Count - (step2 * 2) + (j + 1) % step2
                                });
                }
                var m = CreateMesh(v.ToArray(), u.ToArray(), t.ToArray(), ver => ver.Offset(0,0.9f,0).Rotate(30,0,15,0,0.5f,0), ver => ver.Offset(0, 0.9f, 0).Rotate(30, 0, -15, 0, 0.5f, 0));
                m.name = "sceptreHorns";
                var p = ss.SecretStyle.Structures[1].Element.Prefabs[0].gameObject.CreatePrefab();
                p.name = "slimeEmpathyExoticHorns";
                a = p.GetComponent<SlimeAppearanceObject>();
                a.IgnoreLODIndex = true;
                p.GetComponent<SkinnedMeshRenderer>().sharedMesh = m;
                ss.SecretStyle.Structures[1].Element = CreateElement("slimeEmpathyExoticHorns", a);
                ss.SecretStyle.Structures[1].SupportsFaces = false;

                GenerateBoneData(ss.Id.GetPrefab().GetComponent<SlimeAppearanceApplicator>(), ss.SecretStyle, 0.4f);



                ss = new ModSecretStyle(Ids.REACTOR_SLIME);
                ss.SecretStyle.NameXlateKey = "l.secret_style_reactor";
                ss.SecretStyle.Structures = new[]
                {
                    new SlimeAppearanceStructure(ss.SecretStyle.Structures[0]),
                    new SlimeAppearanceStructure(ss.SecretStyle.Structures[0]),
                    new SlimeAppearanceStructure(ss.SecretStyle.Structures[0])
                };

                ss.SecretStyle.Structures[0].DefaultMaterials = new[] { tabbyMat.Clone() };
                ss.SecretStyle.Structures[0].DefaultMaterials[0].name = "slimeReactorExotic_Screen";
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetTexture("_StripeTexture", null);
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetTexture("_Stripe2Texture", LoadImage("slimeReactorExotic_screenLines.png"));
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetFloat("_StripeSpeed", -59f);
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_TopColor", new Color(0.3f,0.3f,0.3f));
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_MiddleColor", Color.gray);
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_BottomColor", Color.black);
                Main.EditFace(ss.SecretStyle,
                    (new Color(0.15f, 0.75f, 0.9f), new Color(0.15f, 0.75f, 0.9f), new Color(0.5f, 0.25f, 0.3f)),
                    (new Color(0.15f, 0.4f, 0.9f), new Color(0.15f, 0.4f, 0.9f), new Color(0.5f, 0.15f, 0.3f)),
                    "ReactorExotic",
                    (x, y) => {
                        if (x == SlimeFace.SlimeExpression.Angry)
                            x = SlimeFace.SlimeExpression.Grimace;
                        else if (x == SlimeFace.SlimeExpression.AttackTelegraph)
                            x = SlimeFace.SlimeExpression.ChompOpen;
                        else if (x == SlimeFace.SlimeExpression.Blush)
                            x = SlimeFace.SlimeExpression.Elated;
                        else if (x == SlimeFace.SlimeExpression.BlushBlink)
                            x = SlimeFace.SlimeExpression.Blink;
                        else if (x == SlimeFace.SlimeExpression.ChompClosed)
                            x = SlimeFace.SlimeExpression.ChompOpen;
                        else if (x == SlimeFace.SlimeExpression.Glitch)
                            return;
                        else if (x == SlimeFace.SlimeExpression.None)
                            x = SlimeFace.SlimeExpression.Feral;
                        else if (x == SlimeFace.SlimeExpression.Starving)
                            x = SlimeFace.SlimeExpression.Hungry;
                        else if (x == SlimeFace.SlimeExpression.Wince)
                            x = SlimeFace.SlimeExpression.Blink;
                        try
                        {
                            y.SetTexture("_FaceAtlas", LoadImage("slimeReactorExotic_mouth_" + x.ToString().ToLowerInvariant() + ".png", wrapMode: TextureWrapMode.Clamp));
                        }
                        catch (Exception e)
                        {
                            Main.LogError("Missing eye " + x);
                        }
                    },
                    (x, y) => {
                        if (x == SlimeFace.SlimeExpression.Angry)
                            x = SlimeFace.SlimeExpression.Feral;
                        else if (x == SlimeFace.SlimeExpression.AttackTelegraph)
                            x = SlimeFace.SlimeExpression.ChompOpen;
                        else if (x == SlimeFace.SlimeExpression.Blush)
                            x = SlimeFace.SlimeExpression.Blink;
                        else if (x == SlimeFace.SlimeExpression.BlushBlink)
                            x = SlimeFace.SlimeExpression.Blink;
                        else if (x == SlimeFace.SlimeExpression.ChompClosed)
                            x = SlimeFace.SlimeExpression.Blink;
                        else if (x == SlimeFace.SlimeExpression.Glitch)
                            return;
                        else if (x == SlimeFace.SlimeExpression.None)
                            x = SlimeFace.SlimeExpression.Feral;
                        else if (x == SlimeFace.SlimeExpression.Starving)
                            x = SlimeFace.SlimeExpression.Hungry;
                        else if (x == SlimeFace.SlimeExpression.Wince)
                            x = SlimeFace.SlimeExpression.Alarm;
                        try
                        {
                            y.SetTexture("_FaceAtlas", LoadImage("slimeReactorExotic_mouth_" + x.ToString().ToLowerInvariant() + ".png", wrapMode: TextureWrapMode.Clamp));
                        }
                        catch (Exception e)
                        {
                            Main.LogError("Missing mouth " + x);
                        }
                    });
                m = new Mesh();
                m.name = "computerScreen";
                v = new List<Vector3>();
                u = new List<Vector2>();
                t = new List<int>();
                var pX = -0.3f;
                var pY = -0.1f;
                var sX = 0.6f;
                var sY = 0.5f;
                var end = 9;
                int Coord(int x, int y) => x * (end + 1) + y;
                float Arc(int i) => Mathf.Sin((float)i / end * Mathf.PI * 0.5f + (Mathf.PI * 0.25f));
                for (var x = 0; x <= end; x++)
                    for (var y = 0; y <= end; y++)
                    {
                        v.Add(new Vector3(pX + sX * x / end, pY + sY * y / end, 0.8f + Arc(x) * Arc(y) * 0.2f));
                        u.Add(new Vector2((float)x / end, (float)y / end));
                        if (x == 0 || y == 0)
                            continue;
                        t.AddRange(new int[] { Coord(x - 1, y - 1), Coord(x, y - 1), Coord(x - 1, y), Coord(x, y - 1), Coord(x, y), Coord(x - 1, y) });
                    }
                m.vertices = v.ToArray();
                m.uv = u.ToArray();
                m.triangles = t.ToArray();
                m.RecalculateBounds();
                m.RecalculateNormals();
                m.RecalculateTangents();
                p = new GameObject("").CreatePrefab();
                p.name = "computerScreen";
                a = p.AddComponent<SlimeAppearanceObject>();
                a.ParentBone = SlimeAppearance.SlimeBone.JiggleFront;
                a.IgnoreLODIndex = true;
                p.AddComponent<MeshRenderer>();
                p.AddComponent<MeshFilter>().sharedMesh = m;
                ss.SecretStyle.Structures[0].Element = CreateElement("computerScreen", a);

                ss.SecretStyle.Structures[1].DefaultMaterials = new[] { pinkMat.Clone() };
                ss.SecretStyle.Structures[1].DefaultMaterials[0].name = "slimeReactorExotic_Monitor";
                ss.SecretStyle.Structures[1].DefaultMaterials[0].SetColor("_TopColor", new Color(0.8f,0.8f,0.8f));
                ss.SecretStyle.Structures[1].DefaultMaterials[0].SetColor("_MiddleColor", new Color(0.8f, 0.8f, 0.6f));
                ss.SecretStyle.Structures[1].DefaultMaterials[0].SetColor("_BottomColor", new Color(0.5f, 0.5f, 0.5f));
                m = new Mesh();
                m.name = "computerBox";
                v = new List<Vector3>()
                {
                    new Vector3(pX, pY, 1), new Vector3(pX + sX, pY, 1),
                    new Vector3(pX + sX, pY + sY, 1), new Vector3(pX, pY + sY, 1),
                    new Vector3(pX + sX * 0.2f, pY + sY * 0.2f, 0.8f),new Vector3(pX + sX * 0.8f, pY + sY * 0.2f, 0.8f),
                    new Vector3(pX + sX * 0.8f, pY + sY * 0.8f, 0.8f),new Vector3(pX + sX * 0.2f, pY + sY * 0.8f, 0.8f),
                    new Vector3(pX - 0.05f, pY-0.05f, 1.05f), new Vector3(pX + sX+0.05f, pY-0.05f, 1.05f),
                    new Vector3(pX + sX+0.05f, pY + sY+0.05f, 1.05f), new Vector3(pX-0.05f, pY + sY+0.05f, 1.05f),
                    new Vector3(pX - 0.2f, pY-0.2f, 1.05f), new Vector3(pX + sX+0.2f, pY-0.2f, 1.05f),
                    new Vector3(pX + sX+0.2f, pY + sY+0.2f, 1.05f), new Vector3(pX-0.2f, pY + sY+0.2f, 1.05f),
                    new Vector3(pX - 0.25f, pY-0.25f, 1), new Vector3(pX + sX+0.25f, pY-0.25f, 1),
                    new Vector3(pX + sX+0.25f, pY + sY+0.25f, 1), new Vector3(pX-0.25f, pY + sY+0.25f, 1),
                    new Vector3(pX-0.1f, pY-0.1f, 0.3f), new Vector3(pX + sX+0.1f, pY-0.1f, 0.3f),
                    new Vector3(pX + sX+0.1f, pY + sY+0.1f, 0.3f), new Vector3(pX-0.1f, pY + sY+0.1f, 0.3f)
                };
                u = new List<Vector2>();
                t = new List<int>();
                t.AddSquare(1, 5, 4, 0);
                t.AddSquare(3, 7, 6, 2);
                t.AddSquare(0, 4, 7, 3);
                t.AddSquare(2, 6, 5, 1);
                t.AddSquare(4, 5, 6, 7);
                t.AddSquare(9, 1, 0, 8);
                t.AddSquare(11, 3, 2, 10);
                t.AddSquare(8, 0, 3, 11);
                t.AddSquare(10, 2, 1, 9);
                t.AddSquare(13, 9, 8, 12);
                t.AddSquare(15, 11, 10, 14);
                t.AddSquare(12, 8, 11, 15);
                t.AddSquare(14, 10, 9, 13);
                t.AddSquare(17, 13, 12, 16);
                t.AddSquare(19, 15, 14, 18);
                t.AddSquare(16, 12, 15, 19);
                t.AddSquare(18, 14, 13, 17);
                t.AddSquare(21,17, 16,20);
                t.AddSquare(23,19, 18,22);
                t.AddSquare(20,16, 19,23);
                t.AddSquare(22,18, 17,21);
                t.AddSquare(23, 22, 21, 20);
                for (int i = 0; i < v.Count; i++)
                    u.Add(new Vector2(v[i].x, v[i].y));
                m.vertices = v.ToArray();
                m.uv = u.ToArray();
                m.triangles = t.ToArray();
                m.RecalculateBounds();
                m.RecalculateNormals();
                m.RecalculateTangents();
                p = new GameObject("").CreatePrefab();
                p.name = "computerMonitor";
                a = p.AddComponent<SlimeAppearanceObject>();
                a.ParentBone = SlimeAppearance.SlimeBone.JiggleFront;
                a.IgnoreLODIndex = true;
                p.AddComponent<MeshRenderer>();
                p.AddComponent<MeshFilter>().sharedMesh = m;
                ss.SecretStyle.Structures[1].Element = CreateElement("computerScreen", a);
                ss.SecretStyle.Structures[1].SupportsFaces = false;

                Renderer r = Resources.FindObjectsOfTypeAll<ParticleSystemRenderer>().First((x) => x.name == "FX Telespew Sparkle").CreatePrefab();
                r.name = "TelespewSparkles";
                var sm = r.GetComponent<ParticleSystem>().main;
                sm.simulationSpace = ParticleSystemSimulationSpace.World;
                a = r.gameObject.AddComponent<SlimeAppearanceObject>();
                a.ParentBone = SlimeAppearance.SlimeBone.Core;
                a.IgnoreLODIndex = true;
                ss.SecretStyle.Structures[2].Element = CreateElement("slimeReactorExotic_sparkles",a);
                ss.SecretStyle.Structures[2].DefaultMaterials = r.materials;
                ss.SecretStyle.Structures[1].SupportsFaces = false;



                ss = new ModSecretStyle(Ids.AUTOMATA_SLIME);
                ss.SecretStyle.NameXlateKey = "l.secret_style_automata";
                ss.SecretStyle.Structures = new[]
                {
                    new SlimeAppearanceStructure(ss.SecretStyle.Structures[0]),
                    new SlimeAppearanceStructure(ss.SecretStyle.Structures[0]),
                    new SlimeAppearanceStructure(ss.SecretStyle.Structures[0])
                };

                ss.SecretStyle.Structures[0].DefaultMaterials = new[] { tabbyMat.Clone() };
                ss.SecretStyle.Structures[0].DefaultMaterials[0].name = "slimeAutomataExotic_Screen";
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetTexture("_StripeTexture", null);
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetTexture("_Stripe2Texture", LoadImage("slimeReactorExotic_screenLines.png"));
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetFloat("_StripeSpeed", -59f);
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_TopColor", new Color(0.3f, 0.3f, 0.3f));
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_MiddleColor", Color.gray);
                ss.SecretStyle.Structures[0].DefaultMaterials[0].SetColor("_BottomColor", Color.black);
                Main.EditFace(ss.SecretStyle,
                    (new Color(0.15f, 0.75f, 0.9f), new Color(0.15f, 0.75f, 0.9f), new Color(0.5f, 0.25f, 0.3f)),
                    (new Color(0.15f, 0.4f, 0.9f), new Color(0.15f, 0.4f, 0.9f), new Color(0.5f, 0.15f, 0.3f)),
                    "AutomataExotic");


                
            };
        }
    }
    */
    [EnumHolder]
    static class Ids
    {
        public static Identifiable.Id CHRONO_SLIME;
        public static Identifiable.Id MATTER_SLIME;
        public static Identifiable.Id AUTOMATA_SLIME;
        public static Identifiable.Id EMPATHY_SLIME;
        public static Identifiable.Id REACTOR_SLIME;
    }

    [EnumHolder]
    static class Ids2
    {
        public static PediaDirector.Id CHRONO_SLIME;
        public static PediaDirector.Id MATTER_SLIME;
        public static PediaDirector.Id AUTOMATA_SLIME;
        public static PediaDirector.Id EMPATHY_SLIME;
        public static PediaDirector.Id REACTOR_SLIME;
    }

    [SRML.Config.Attributes.ConfigFile("spawnSettings")]
    public static class Config
    {
        public static int spawnChance = 100;
    }

    class TimeWarp : SRBehaviour, ExtendedData.Participant
    {
        List<Position> prevPositions = new List<Position>();
        double lastTeleportTime = SceneContext.Instance ? SceneContext.Instance.TimeDirector.HoursFromNow(-TeleportDelay) : 0;
        SlimeEmotions emotions;
        Rigidbody body;
        static float TeleportDelay = 0.167f;
        void Awake()
        {
            emotions = GetComponent<SlimeEmotions>();
            body = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if ((prevPositions.Count == 0 || (prevPositions.Last().location - transform.position).sqrMagnitude > 400) && body.velocity.sqrMagnitude < 4)
            {
                prevPositions.Add(new Position(this));
            }
            if (prevPositions.Count > 30)
                prevPositions.RemoveRange(0, prevPositions.Count - 30);
            if (-SceneContext.Instance.TimeDirector.HoursUntil(lastTeleportTime) > TeleportDelay && (emotions.Unhappiness() > 0.9f || transform.position.y < -5.5))
            {
                body.velocity = Vector3.zero;
                if (prevPositions.Count > 1)
                {
                    prevPositions[prevPositions.Count - 2].MoveTo(transform);
                    prevPositions.RemoveAt(prevPositions.Count - 1);
                }
                else
                    prevPositions[0].MoveTo(transform);
                lastTeleportTime = SceneContext.Instance.TimeDirector.WorldTime();
            }
        }

        public void ReadData(CompoundDataPiece data)
        {
            if (data.HasPiece("lastTeleport"))
                lastTeleportTime = data.GetValue<double>("lastTeleport");
            if (data.HasPiece("positions"))
            {
                var data2 = data.GetCompoundPiece("positions");
                prevPositions.Clear();
                foreach (CompoundDataPiece v in data2.DataList)
                    prevPositions.Add(new Position(v.GetValue<string>("parent"), v.GetValue<Vector3>("location"), v.GetValue<Quaternion>("rotation")));
            }
        }

        public void WriteData(CompoundDataPiece data)
        {
            data.SetValue("lastTeleport", lastTeleportTime);
            int i = 0;
            var data2 = new CompoundDataPiece("positions");
            foreach (var p in prevPositions)
            {
                var data3 = new CompoundDataPiece(i++.ToString());
                data3.SetValue("parent", p.parentPath);
                data3.SetValue("location", p.location);
                data3.SetValue("rotation", p.rotation);
                data2.AddPiece(data3);
            }
            data.AddPiece(data2);
        }


        class Position
        {
            public string parentPath;
            public Vector3 location;
            public Quaternion rotation;
            public Position(Behaviour component) : this(component.transform) { }
            public Position(Transform transform)
            {
                Transform p = transform.parent;
                parentPath = "";
                while (p)
                {
                    parentPath = p.name = parentPath.Length > 0 ? "/" + parentPath : "";
                    p = p.parent;
                }
                location = transform.position;
                rotation = transform.rotation;
            }
            public Position(string ParentPath, Vector3 Location, Quaternion Rotation)
            {
                parentPath = ParentPath;
                location = Location;
                rotation = Rotation;
            }
            public void MoveTo(Transform transform)
            {
                var gO = GameObject.Find(parentPath);
                if (gO)
                    transform.SetParent(gO.transform);
                else
                    transform.SetParent(null);
                transform.position = location;
                transform.rotation = rotation;
            }
        }
    }

    class ClockHandAnimator : MonoBehaviour
    {
        float time = 0;
        SlimeEmotions emo;
        void Start() => emo = GetComponentInParent<SlimeEmotions>();
        void Update()
        {
            time += Time.deltaTime * (1 + (emo ? emo.Unhappiness() * 5 : 0)) / 2;
            if (time > 12)
                time %= 12;
            transform.GetChild(0).localRotation = Quaternion.Euler(0, 360 * time, 0);
            transform.GetChild(1).localRotation = Quaternion.Euler(0, 360 * time / 12, 0);
        }
    }

    class IdleSpawner : MonoBehaviour
    {
        float cycleTime = 2;
        float time = 0;
        public GameObject fx;
        List<GameObject> spawned = new List<GameObject>();
        void Start() => time = cycleTime;
        void Update()
        {
            time += Time.deltaTime;
            if (time > cycleTime)
            {
                time %= cycleTime;
                spawned.Add(SRBehaviour.SpawnAndPlayFX(fx, gameObject));
            }
            spawned.RemoveAll(x => !x);
        }
        void OnDisable()
        {
            foreach (var g in spawned)
                Destroy(g);
            spawned.Clear();
        }
    }

    class Empathize : MonoBehaviour
    {
        float time = 0;
        SlimeEmotions emo;
        bool canFeral = false;
        void Awake()
        {
            emo = GetComponent<SlimeEmotions>();
            canFeral = GetComponent<SlimeFeral>() && GetComponent<SlimeFeral>().enabled;
        }
        void Update()
        {
            time += Time.deltaTime;
            if (time > 1)
            {
                time %= 1;
                foreach (var col in Physics.OverlapSphere(transform.position, 4)) {
                    var otherEmo = col.GetComponentInParent<SlimeEmotions>();
                    if (otherEmo && otherEmo != emo && otherEmo.enabled)
                    {
                        otherEmo.model.emotionAgitation.currVal = emo.model.emotionAgitation.currVal;
                        otherEmo.model.emotionFear.currVal = emo.model.emotionFear.currVal;
                        if (canFeral && otherEmo.GetComponent<SlimeFeral>())
                            otherEmo.model.isFeral = emo.model.isFeral;
                    }
                }
            }
        }
    }

    class DisableEmotions : MonoBehaviour
    {
        public SlimeEmotions.Emotion[] emotions = new SlimeEmotions.Emotion[0];
        SlimeEmotions emo;
        void Awake() => emo = GetComponent<SlimeEmotions>();
        void Update()
        {
            if (!emo)
            {
                DestroyImmediate(this);
                return;
            }
            foreach (var emote in emo.model.allEmotions)
                if (emotions.Contains(emote.emotion))
                    emote.currVal = emote.defVal;
        }
    }

    class OrbitRandom : MonoBehaviour
    {
        Vector3 tarEuler;
        float tarSpeed;
        Vector3 currEuler;
        float currSpeed;
        float time = 0;
        public float minDistance = 1;
        public float maxDistance = 2;
        void Start() => Randomize();
        void Update()
        {
            time += Time.deltaTime;
            if (time >= 1)
            {
                time %= 1;
                Randomize();
            }
            transform.localPosition = Vector3.forward.Rotate(Vector3.Lerp(currEuler, tarEuler, time)) * Mathf.Lerp(currSpeed,tarSpeed,time);
        }
        void Randomize()
        {
            currEuler = tarEuler;
            currSpeed = tarSpeed;
            tarEuler = Quaternion.AngleAxis(Random.Range(0f, 360), new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized).eulerAngles;
            tarSpeed = Random.Range(minDistance, maxDistance);
        }
    }

    class SlimeEatEnergy : MonoBehaviour
    {
        double time;
        public EnergyCollector eaterPrefab;
        EnergyCollector eater;
        SlimeEmotions emo;
        void Start()
        {
            emo = GetComponent<SlimeEmotions>();
            eater = Instantiate(eaterPrefab, transform, false);
        }
        void Update()
        {
            time += SceneContext.Instance.TimeDirector.DeltaWorldTime();
            if (time >= 60)
            {
                var delta = (int)(time / 60);
                time %= 60;
                var want = emo.GetCurr(SlimeEmotions.Emotion.HUNGER);
                foreach (var o in eater.GetObjects())
                {
                    var r = o.GetComponent<RadSource>();
                    if (r && r.isActiveAndEnabled)
                        want -= r.radPerSecond / 1000 * delta;
                    var b = o.GetComponent<BoomSlimeExplode>();
                    if (b && b.isActiveAndEnabled && b.state == BoomSlimeExplode.State.IDLE)
                    {
                        want -= b.GetReadiness() * b.maxPlayerDamage / 100;
                        b.nextPossibleExplode += b.GetReadiness() * b.nextExplodeDelayTime;
                    }
                    var i = o.GetComponent<FireSlimeIgnition>();
                    if (i && i.isActiveAndEnabled && i.isIgnited)
                    {
                        want -= 0.3f;
                        i.Extinguish();
                    }
                    var s = o.GetComponent<ReactToShock>();
                    if (s && s.isActiveAndEnabled && !s.damagePlayer.blocked)
                        want -= s.damagePlayer.damagePerTouch / 100000 * delta / s.damagePlayer.repeatTime == 0 ? s.damagePlayer.repeatTime : 0.01f;
                    foreach (var supply in o.GetComponents<IEnergySupplier>())
                        if (supply.IsSupplyActive())
                            want -= supply.TakeEnergy();
                    foreach (var pair in EnergyRegistry.suppliers)
                        foreach (var supplier in o.GetComponents(pair.Key))
                            if (pair.Value.Item1(supplier))
                                want -= pair.Value.Item2(supplier);
                }
                emo.model.emotionHunger.currVal = want < 0 ? 0 : want;
                var scale = transform.lossyScale;
                var rad = Mathf.Max(scale.x, scale.y, scale.z) * 20;
                rad *= rad;
                if (want < 0)
                {
                    want = -want * 100;
                    if ((SceneContext.Instance.Player.transform.position - transform.position).sqrMagnitude < rad)
                    {
                        var missing = (int)Mathf.Min(SceneContext.Instance.PlayerState.GetMaxEnergy() - SceneContext.Instance.PlayerState.GetCurrEnergy(), want);
                        SceneContext.Instance.PlayerState.SetEnergy(SceneContext.Instance.PlayerState.GetCurrEnergy() + missing);
                        want -= missing;
                        if (want < 1)
                            return;
                    }
                    foreach (var obj in eater.GetObjects())
                    {
                        var otherEnergyEat = obj.GetComponent<SlimeEatEnergy>();
                        if (otherEnergyEat && otherEnergyEat != this)
                        {
                            var missing = Mathf.Min(1 - otherEnergyEat.emo.GetCurr(SlimeEmotions.Emotion.HUNGER) * 100, want);
                            otherEnergyEat.emo.model.emotionHunger.currVal += missing / 100;
                            want -= missing;
                            if (want < 1)
                                return;
                        }
                        var battery = obj.GetComponentInParent<DroneStationBattery>();
                        if (battery)// && (battery.transform.position - transform.position).sqrMagnitude < rad)
                        {
                            var missing = Math.Min(Math.Min(Math.Max(1 - (battery.Time - SceneContext.Instance.TimeDirector.WorldTime()) / DroneStationBattery.DURATION_SECONDS, 0), 1) * DroneStationBattery.DURATION_HOURS * 40, want);
                            battery.droneModel.batteryDepleteTime = Math.Max(battery.droneModel.batteryDepleteTime, SceneContext.Instance.TimeDirector.WorldTime()) + missing / 40 / DroneStationBattery.DURATION_HOURS * DroneStationBattery.DURATION_SECONDS;
                            want -= (float)missing;
                            if (want < 1)
                                return;
                        }
                        foreach (var reciever in obj.GetComponents<IEnergyReciever>())
                        {
                            var missing = Mathf.Min(reciever.MissingEnergy(), want);
                            reciever.AddEnergy(missing);
                            want -= missing;
                        }
                        foreach (var pair in EnergyRegistry.recievers)
                            foreach (var reciever in obj.GetComponents(pair.Key))
                            {
                                var missing = Mathf.Min(pair.Value.Item1(reciever), want);
                                pair.Value.Item2(reciever, missing);
                                want -= missing;
                            }
                    }
                }
            }
        }
    }

    public interface IEnergyReciever
    {
        float MissingEnergy();
        void AddEnergy(float energy);
    }

    public interface IEnergySupplier
    {
        bool IsSupplyActive();
        float TakeEnergy();
    }

    public static class EnergyRegistry
    {
        internal static Dictionary<Type, System.Tuple<Func<object, float>, Action<object, float>>> recievers = new Dictionary<Type, System.Tuple<Func<object, float>, Action<object, float>>>();
        internal static Dictionary<Type, System.Tuple<Func<object, bool>, Func<object, float>>> suppliers = new Dictionary<Type, System.Tuple<Func<object, bool>, Func<object, float>>>();
        public static void RegisterReceiver<T>(Func<T, float> MissingEnergy, Action<T, float> AddEnergy) where T : Behaviour
        {
            if (MissingEnergy == null)
                throw new ArgumentNullException("MissingEnergy");
            if (AddEnergy == null)
                throw new ArgumentNullException("AddEnergy");
            recievers[typeof(T)] = new System.Tuple<Func<object, float>, Action<object, float>>(
                (x) => MissingEnergy((T)x),
                (x, y) => AddEnergy((T)x, y)
            );
        }
        public static void DeregisterReceiver<T>() where T : Behaviour => recievers.Remove(typeof(T));
        public static void RegisterSupplier<T>(Func<T, bool> IsSupplyActive, Func<T, float> TakeEnergy) where T : Behaviour
        {
            if (IsSupplyActive == null)
                throw new ArgumentNullException("MissingEnergy");
            if (TakeEnergy == null)
                throw new ArgumentNullException("AddEnergy");
            suppliers[typeof(T)] = new System.Tuple<Func<object, bool>, Func<object, float>>(
                (x) => IsSupplyActive((T)x),
                (x) => TakeEnergy((T)x)
            );
        }
        public static void DeregisterSupplier<T>() where T : Behaviour => suppliers.Remove(typeof(T));
    }

    class EnergyCollector : MonoBehaviour
    {
        List<GameObject> contains = new List<GameObject>();
        void OnTriggerEnter(Collider other)
        {
            if (other)
                contains.AddUnique(other.gameObject);
        }
        void OnTriggerExit(Collider other)
        {
            if (other)
                contains.Remove(other.gameObject);
        }
        public GameObject[] GetObjects()
        {
            contains.RemoveAll((x) => !x);
            return contains.ToArray();
        }
    }

    [HarmonyPatch(typeof(ResourceBundle), "LoadFromText")]
    class Patch_LoadResources
    {
        static void Postfix(string path, Dictionary<string, string> __result)
        {
            var lang = GameContext.Instance.MessageDirector.GetCultureLang();
            if (path == "actor")
            {
                var chrono = Ids.CHRONO_SLIME.ToString().ToLowerInvariant();
                var matter = Ids.MATTER_SLIME.ToString().ToLowerInvariant();
                var automata = Ids.AUTOMATA_SLIME.ToString().ToLowerInvariant();
                var empathy = Ids.EMPATHY_SLIME.ToString().ToLowerInvariant();
                var reactor = Ids.REACTOR_SLIME.ToString().ToLowerInvariant();
                if (lang == MessageDirector.Lang.RU)
                {
                    __result["l." + chrono] = "Слайм времени";
                    __result["l." + matter] = "Слайм-Материя";
                    __result["l." + automata] = "Слайм-Автоматон";
                    __result["l." + empathy] = "Слайм эмпатии";
                    __result["l." + reactor] = "Слайм-Реактор";
                }
                else
                {
                    __result["l." + chrono] = "Chrono Slime";
                    __result["l." + matter] = "Matter Slime";
                    __result["l." + automata] = "Automata Slime";
                    __result["l." + empathy] = "Empathy Slime";
                    __result["l." + reactor] = "Reactor Slime";
                }
                /*__result["l.secret_style_" + chrono] = "";
                __result["l.secret_style_" + matter] = "";
                __result["l.secret_style_" + automata] = "";
                __result["l.secret_style_" + empathy] = "Sceptre";
                __result["l.secret_style_" + reactor] = "Process";*/
            }
            else if (path == "pedia")
            {
                var chrono = Ids2.CHRONO_SLIME.ToString().ToLowerInvariant();
                var matter = Ids2.MATTER_SLIME.ToString().ToLowerInvariant();
                var automata = Ids2.AUTOMATA_SLIME.ToString().ToLowerInvariant();
                var empathy = Ids2.EMPATHY_SLIME.ToString().ToLowerInvariant();
                var reactor = Ids2.REACTOR_SLIME.ToString().ToLowerInvariant();
                if (lang == MessageDirector.Lang.RU)
                {
                    __result.Add(chrono, "Слайм времени",
                        "Может летать, когда становится слишком веселым",
                        "Любит заменять объекты возможными альтернативами из прошлого. Поведение этого слайма предполагает, что временные линии могут разделятся на 2 ветки в разных направлениях, и один момент будет точкой нахождения разных объектов, в то же время будучи точкой разветвления для возможных вариантов будущего",
                        "Насколько известно, он не производит плорты",
                        "(нет)",
                        "Когда голоден, может отправится в прошлое, сбегая из загона и подделывая объекты где угодно",
                        "Плорты");

                    __result.Add(matter, "Слайм-Материя",
                        "Реальность-что угодно, чего он хочет",
                        "Любит играться с трансмутацией. Похоже, не понимает концепта \"Невозможности\"",
                        "Насколько известно, он не производит уникальных плортов",
                        "(нет)",
                        "Во время еды может произвести абсолютно любой объект. Лучше следить за ним, чтобы вещи не вышли из-под контроля",
                        "Всё");

                    __result.Add(automata, "Слайм-Автоматон",
                        "Автоматон в форме слайма",
                        "Этот механизм полагается на энергию других созданий, дабы существовать самому",
                        "Этот слайм не производит плортов, но существа, из которых он забирает жизненную силу, быстро стареют",
                        "(нет)",
                        "Обычно он старается брать энергию у существ с малым интеллектом, но при слишком сильном голоде он будет поглощать энергию из всего, из чего может",
                        "Жизненная сила");

                    __result.Add(empathy, "Слайм эмпатии",
                        "Есть эмпатия, но не симпатия",
                        "Слаймы вокруг него замечены за имитацией его настроения, словно он контроллирует их разум. Это заставило многих исследователей слаймов поверить в то, что тот владеет некоторой формой телепатии, позволяющей ему заставлять других слаймов чувствовать то же самое, что и он сам",
                        "Насколько известно, он не производит плорты",
                        "(нет)",
                        "Несмотря на то, что ему не нужна еда, у него все еще есть собственное настроение, частично зависящее от других слаймов и влияющее на них. Радуйте его, и все будут счастливы. Злите его, и не ждите что другие слаймы будут относится к вам с теплотой",
                        "(нет)");

                    __result.Add(reactor, "Слайм-Реактор",
                        "Он очень энергичный!",
                        "Берет энергию у других слаймов и распространяет в окружающую среду. Это может стать полезным дополнением для исследовательских цехов в качестве природного источника энергии",
                        "Насколько известно, он не производит плорты",
                        "(нет)",
                        "Нет известных опасностей",
                        "Энергия");
                }
                else
                {
                    __result.Add(chrono, "Chrono Slime",
                        "Might fly when it gets too happy",
                        "Likes to replace objects with possible alternatives from prior points in time. This slime's behaviour suggests that the timelines may branch in 2 directions, a single moment being a convergence point for multiple pasts as well as being a branch point for the possible futures",
                        "As far as is known, it does not produce any plorts",
                        "(none)",
                        "When unhappy it may decide to move itself back in time. This can risk leaving it's corral and tampering with objects elsewhere",
                        "Plorts");

                    __result.Add(matter, "Matter Slime",
                        "Reality is whatever it wants it to be",
                        "Likes to mess with transmutation. It does not seem to understand the concept of something being \"Impossible\"",
                        "As far as is known, it does not produce any unique plorts",
                        "(none)",
                        "When eating, this slime can produce all manner of objects. Best to keep an eye on it to make sure things stay under control",
                        "Everything");

                    __result.Add(automata, "Automata Slime",
                        "An automaton in slime form",
                        "This mechanism relies on the life of creatures to sustain it's own",
                        "This slime does not produce plorts however objects it takes life force from tend to age rapidly while it does so",
                        "(none)",
                        "While it's normally careful to only take life force from creatures lacking intelligence, if it becomes desprate enough it may decide to take from anything it can get it's hands on",
                        "Life Force");

                    __result.Add(empathy, "Empathy Slime",
                        "Has empathy but not sympathy",
                        "Slimes around it have been obsevered mimicing it's temperment, almost like it's mind controlling them. This has lead many Slime researchers to beleive it possess a simple form of telepathy that allows it to force other slimes to empathise with it.",
                        "As far as is known, it does not produce any plorts",
                        "(none)",
                        "While it does not seem to need food, it's ability to affect the attitude of nearby slimes can be both good and bad, depending on it's own mood. Keep it happy and it will keep the others happy. Upset it and risk anger from the other slimes on its behalf",
                        "(none)");

                    __result.Add(reactor, "Reactor Slime",
                        "Is very energetic",
                        "Consumes energy from nearby slimes and disperces the extra into the surrounding area. This has become a useful addition to many research facilities as a natural source of energy",
                        "As far as is known, it does not produce any plorts",
                        "(none)",
                        "No known risks",
                        "Energy");
                }
            }
        }
    }

    [HarmonyPatch(typeof(SlimeEat), "EatAndTransform")]
    class Patch_SlimeEatTransform
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = instructions.ToList();
            var ind = code.FindIndex((x) => x.opcode == OpCodes.Call && x.operand is MethodInfo && (x.operand as MethodInfo).DeclaringType == typeof(SRBehaviour) && (x.operand as MethodInfo).Name == "InstantiateActor") + 2;
            code.Insert(ind++, new CodeInstruction(OpCodes.Ldarg_0));
            code.Insert(ind++, new CodeInstruction(OpCodes.Ldloc_1));
            code.Insert(ind++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_SlimeEatTransform), nameof(Modify))));
            return code;
        }
        static void Modify(SlimeEat original, GameObject newSlime)
        {
            var t1 = original.GetComponent<TimeWarp>();
            var t2 = newSlime.GetComponent<TimeWarp>();
            if (!t1 || !t2)
                return;
            var data = new CompoundDataPiece("");
            t1.WriteData(data);
            t2.ReadData(data);
        }
    }

    [HarmonyPatch(typeof(SlimeEat),"EatAndProduce")]
    class Patch_SlimeEatProduce
    {
        static bool Prefix(SlimeEat __instance, SlimeDiet.EatMapEntry em)
        {
            if (__instance.slimeDefinition.Matches(Ids.CHRONO_SLIME))
            {
                var maps = new List<Identifiable.Id>();
                foreach (var d in GameContext.Instance.SlimeDefinitions.slimeDefinitionsByIdentifiable.Values)
                    if (!Main.ChronoDietIgnore.Exists(x => d.Matches(x)) && d.Diet != null)
                        foreach (var mapValues in d.Diet.EatMap)
                            if (mapValues.producesId == em.eats)
                                for (int i = 0; i < mapValues.NumToProduce(); i++)
                                    maps.Add(mapValues.eats);
                if (maps.Count == 0)
                    return false;
                em.producesId = maps.RandomObject();
            }
            if (__instance.slimeDefinition.Matches(Ids.MATTER_SLIME))
            {
                var maps = new List<Identifiable.Id>();
                foreach (var d in GameContext.Instance.LookupDirector.identifiablePrefabs)
                {
                    var id = d.GetComponent<Identifiable>();
                    if (id && id.id != Identifiable.Id.NONE)
                        maps.Add(id.id);
                }
                if (maps.Count == 0)
                    return false;
                em.producesId = maps.RandomObject();
            }
            if (__instance.slimeDefinition.Matches(Ids.AUTOMATA_SLIME))
            {
                if (em.eats != Identifiable.Id.PLAYER)
                {
                    var prefab = GameContext.Instance.LookupDirector.GetPrefab(em.eats);
                    var transformTime = prefab.GetComponent<TransformAfterTime>();
                    var transformChance = prefab.GetComponent<TransformChanceOnReproduce>();
                    if (transformTime || transformChance)
                    {
                        var option = transformTime ? transformTime.options.RandomObject((x) => x.weight).targetPrefab : transformChance.targetPrefab;
                        if (option == null)
                            return false;
                        em.producesId = option.GetComponent<Identifiable>().id;
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SlimeDiet), "RefreshEatMap")]
    class Patch_RefreshEat
    {
        static void Postfix(SlimeDiet __instance, SlimeDefinitions definitions, SlimeDefinition definition)
        {
            if (definition.Matches(Ids.CHRONO_SLIME))
            {
                var added = __instance.EatMap.All((x) => x.producesId != Identifiable.Id.NONE,(x) => x.producesId, true);
                foreach (var d in definitions.slimeDefinitionsByIdentifiable.Values)
                    if (!Main.ChronoDietIgnore.Exists(x => d.Matches(x)) && d.Diet != null && d.Diet.EatMap != null)
                        foreach (var em in d.Diet.EatMap)
                            if (!Main.ChronoEatIgnore.Contains(em.eats) && em.producesId != Identifiable.Id.NONE && !added.Contains(em.producesId))
                            {
                                __instance.EatMap.Add(new SlimeDiet.EatMapEntry()
                                {
                                    driver = SlimeEmotions.Emotion.HUNGER,
                                    eats = em.producesId,
                                    extraDrive = 0.3f,
                                    minDrive = 0,
                                    producesId = Ids.CHRONO_SLIME
                                });
                                added.Add(em.producesId);
                            }
            }
            if (definition.Matches(Ids.AUTOMATA_SLIME))
            {
                var animals = Identifiable.CHICK_CLASS.Concat(Identifiable.MEAT_CLASS).ToList();
                var animals2 = GameContext.Instance.LookupDirector.identifiablePrefabs.All((x) => x.GetComponent<Identifiable>() ? animals.Contains(x.GetComponent<Identifiable>().id) && (x.GetComponent<TransformAfterTime>() || x.GetComponent<TransformChanceOnReproduce>()) : false, (x) => x.GetComponent<Identifiable>().id);
                foreach (var t in animals2)
                    __instance.EatMap.Add(new SlimeDiet.EatMapEntry()
                    {
                        driver = SlimeEmotions.Emotion.HUNGER,
                        eats = t,
                        extraDrive = 0.5f,
                        minDrive = 0,
                        producesId = Identifiable.Id.GOLD_ECHO
                    });
                foreach (var t in animals)
                    if (!animals2.Contains(t))
                    __instance.EatMap.Add(new SlimeDiet.EatMapEntry()
                    {
                        driver = SlimeEmotions.Emotion.HUNGER,
                        eats = t,
                        extraDrive = -0.4f,
                        producesId = Identifiable.Id.WATER_LIQUID
                    });
                __instance.EatMap.Add(new SlimeDiet.EatMapEntry()
                    {
                        driver = SlimeEmotions.Emotion.HUNGER,
                        eats = Identifiable.Id.PLAYER,
                        extraDrive = -0.4f,
                        producesId = Identifiable.Id.WATER_LIQUID
                    });
            }
        }
    }

    [HarmonyPatch(typeof(Chomper), "StartChomp")]
    class Patch_StartChomp
    {
        static void Prefix(Chomper __instance, GameObject other, Identifiable.Id otherId, ref Chomper.OnChompCompleteDelegate onChompComplete)
        {
            var def = GameContext.Instance.SlimeDefinitions.GetSlimeByIdentifiableId(otherId);
            if (def && def.Matches(Ids.MATTER_SLIME))
            {
                onChompComplete = (w,x,y,z) => { };
                __instance.StartCoroutine(DestroyCoroutine(__instance.transform));
            }
        }
        static IEnumerator DestroyCoroutine(Transform transform)
        {
            float endTime = 0.5f;
            float time = 0;
            Vector3 start = transform.localScale;
            Vector3 end = Vector3.one * 0.1f;
            while (time < endTime)
            {
                transform.localScale = Vector3.Lerp(start, end, time / endTime);
                time += Time.deltaTime;
                yield return null;
            }
            Destroyer.DestroyActor(transform.gameObject, "Patch_StartChomp.DestroyCoroutine");
            yield break;
        }
    }

    [HarmonyPatch(typeof(DirectedSlimeSpawner), "MaybeReplacePrefab")]
    class Patch_SlimeSpawnerPrefab
    {
        static void Postfix(DirectedSlimeSpawner __instance, ref GameObject __result)
        {
            var zone = __instance.region.GetZoneId();
            foreach (var p in Main.SpawnArea)
                if (zone == p.Value && Randoms.SHARED.GetProbability(1f / Config.spawnChance))
                    __result = GameContext.Instance.LookupDirector.GetPrefab(p.Key);
        }
    }

    [HarmonyPatch(typeof(ExchangeDirector), "Awake")]
    class Patch_ExchangeStart
    {
        public static void Prefix(ExchangeDirector __instance)
        {
            __instance.values = __instance.values.AddRangeToArray(new ExchangeDirector.ValueEntry[] {
                new ExchangeDirector.ValueEntry()
                {
                    id = Ids.AUTOMATA_SLIME,
                    value = 75
                },
                new ExchangeDirector.ValueEntry()
                {
                    id = Ids.CHRONO_SLIME,
                    value = 75
                },
                new ExchangeDirector.ValueEntry()
                {
                    id = Ids.EMPATHY_SLIME,
                    value = 75
                },
                new ExchangeDirector.ValueEntry()
                {
                    id = Ids.MATTER_SLIME,
                    value = 75
                },
                new ExchangeDirector.ValueEntry()
                {
                    id = Ids.REACTOR_SLIME,
                    value = 75
                }
            });
        }
    }

    static class ExtentionMethods
    {
        public static List<Y> All<X,Y>(this IEnumerable<X> c, Predicate<X> predicate, Func<X,Y> converter, bool enforceUnique = false)
        {
            var l = new List<Y>();
            foreach (var i in c)
                if (predicate(i))
                {
                    if (enforceUnique)
                        l.AddUnique(converter(i));
                    else
                        l.Add(converter(i));
                }
            return l;
        }

        public static List<Y> All<X, Y>(this IEnumerable<X> c, Predicate<X> predicate, Func<X, IEnumerable<Y>> converter, bool enforceUnique = false)
        {
            var l = new List<Y>();
            foreach (var i in c)
                if (predicate(i))
                {
                    if (enforceUnique)
                        l.AddRangeUnique(converter(i));
                    else
                        l.AddRange(converter(i));
                }
            return l;
        }
        public static bool Matches(this SlimeDefinition a, SlimeDefinition b) => a == b || (a.BaseSlimes != null && a.BaseSlimes.Contains(b)) || (b.BaseSlimes != null && b.BaseSlimes.Contains(a));
        public static bool Matches(this SlimeDefinition a, Identifiable.Id id) => a.IdentifiableId == id || (a.BaseSlimes != null && a.BaseSlimes.Any((x) => x.IdentifiableId == id));
        public static float Unhappiness(this SlimeEmotions emotions) => Mathf.Max(emotions.GetCurr(SlimeEmotions.Emotion.AGITATION), emotions.GetCurr(SlimeEmotions.Emotion.HUNGER), emotions.GetCurr(SlimeEmotions.Emotion.FEAR));

        public static void AddSquare(this List<int> t, int a, int b, int c, int d) => t.AddRange(new int[] { a, b, d, b, c, d });
        public static void AddSquare(this List<int> t, List<Vector3> v, Vector3 a, Vector3 b, Vector3 c, Vector3 d) => t.AddSquare(v.AddOrGetIndex(a), v.AddOrGetIndex(b), v.AddOrGetIndex(c), v.AddOrGetIndex(d));

        public static int AddOrGetIndex<T>(this List<T> t, T value)
        {
            var i = t.IndexOf(value);
            if (i == -1)
            {
                i = t.Count;
                t.Add(value);
            }
            return i;
        }

        public static void Add(this Dictionary<string,string> col, string key, string name, string intro, string ology, string plort, string favou, string risks, string diets)
        {
            col["t." + key] = name;
            col["m.intro." + key] = intro;
            col["m.slimeology." + key] = ology;
            col["m.plortonomics." + key] = plort;
            col["m.favorite." + key] = favou;
            col["m.risks." + key] = risks;
            col["m.diet." + key] = diets;
        }
    }
}