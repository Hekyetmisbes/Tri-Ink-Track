# Kurulum ve Calistirma

## Gereksinimler

- Unity Editor `6000.3.9f1`
- Windows + .NET destekli Unity kurulumu
- Input System paketi (manifestte mevcut)

## Projeyi Acma

1. Unity Hub -> Add Project -> `TriInkTrack` klasorunu sec.
2. Editor surumunu `6000.3.9f1` ile ac.
3. `Assets/_Game/Scenes/GameScene.unity` sahnesini ac.

## Oynatma

1. Play tusuna bas.
2. Mouse/touch ile cizgi ciz.
3. UI butonlari ile `Ice/Sticky/Bouncy` sec.
4. Topu hedefe ulastir.

## Hizli Onarim / Yeniden Kurulum

Editor menu araclari:

- `Tools/TriInkTrack/Step 2/Build Ball+Target Setup`
- `Tools/TriInkTrack/Step 3/Build DrawSystem Setup`
- `Tools/TriInkTrack/Step 5/Build Ink Types Setup`

Bu menuler, sahne ve prefab baglantilarini yeniden kurmak icin kullanilir.

## Sorun Giderme

1. Cizim calismiyorsa:
   - EventSystem ve input module ayarlarini kontrol et.
   - `DrawSystem` referanslarinin dolu oldugunu kontrol et.
2. Murekkep meter guncellenmiyorsa:
   - `InkSelectionUI` -> `inkInventory` baglantisini kontrol et.
3. Cizgiler fizik vermiyorsa:
   - `InkLine` prefabinda `Ice/Sticky/Bouncy` fizik materyallerini kontrol et.

## Not

Proje su an prototip odakli oldugu icin, once gameplay stabilitesi ve level akisinin tamamlanmasi onerilir.
