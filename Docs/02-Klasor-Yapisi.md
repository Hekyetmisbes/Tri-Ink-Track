# Klasor Yapisi

## Kok Dizin

- `Assets/`: Oyun icerigi ve kod
- `Packages/`: Unity paket bagimliliklari
- `ProjectSettings/`: Proje ayarlari
- `Docs/`: Teknik ve surec dokumanlari

## `Assets/_Game` Yapisi

- `Scenes/`
  - `GameScene.unity` (ana ve tek gameplay sahnesi)
- `Scripts/`
  - `Core/`
    - `GameManager.cs`
    - `LevelRoot.cs`
  - `Ball/`
    - `BallController.cs`
  - `Target/`
    - `TargetGoal.cs`
  - `Drawing/`
    - `DrawSystem.cs`
    - `InkLine.cs`
    - `InkLineLifetime.cs`
    - `InkLinePool.cs`
  - `Ink/`
    - `InkInventory.cs`
    - `InkType.cs`
  - `UI/`
    - `InkSelectionUI.cs`
  - `Config/`
    - `GameConfig.cs`
  - `Editor/`
    - `Step2SetupTool.cs`
    - `Step3SetupTool.cs`
    - `Step5SetupTool.cs`
- `Prefabs/`
  - `Ball.prefab`
  - `Target.prefab`
  - `Wall.prefab`
  - `InkLine.prefab`
- `ScriptableObjects/`
  - `GameConfig.asset`
- `PhysicsMaterials/`
  - `Ice.physicsMaterial2D`
  - `Sticky.physicsMaterial2D`
  - `Bouncy.physicsMaterial2D`

## Ucuncu Parti / Hazir Assetler

- `Assets/kenney_*` paketleri (grafik/ses kaynaklari)
- `Assets/UnityTechnologies/ParticlePack` (ornek efekt paketleri)

Not: Ucuncu parti asset miktari yuksek oldugu icin, aktif oyun icerigi `_Game` altinda tutuluyor. Bu ayrim korunmali.
