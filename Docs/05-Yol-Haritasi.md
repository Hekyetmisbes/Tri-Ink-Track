# Yol Haritasi

## Faz 1 - Oynanis Kararliligi (Kisa Vade)

1. `Fail` kosullarini netlestir ve uygula.
2. `Retry` akisini sahneyi/leveli guvenli resetleyecek sekilde tamamla.
3. Input pipeline'i tek standarda cek (`InputSystemUIInputModule` vs `StandaloneInputModule`).
4. UI buton listener tekrarlarini temizle ve dogrula.

Basari kriteri:
- 10+ denemede kazanma/kaybetme/reset dongusu stabil calismali.

## Faz 2 - Level Sistemi (Orta Vade)

1. Level tanimini ScriptableObject ya da prefab tabanli standarda cek.
2. `NextLevel` gercek level gecisine donussun.
3. En az 5 farkli zorlukta level ekle.
4. Basit level secim UI ve ilerleme gostergesi ekle.

Basari kriteri:
- Oyuncu 1->N seviye akisini kesintisiz tamamlayabilmeli.

## Faz 3 - Kalite ve Test (Orta/Uzun Vade)

1. Cekirdek mekaniklere PlayMode testleri yaz:
   - Ink tuketimi
   - Win/Fail state gecisleri
   - Cizgi omru ve pool donusu
2. Kritik helper siniflarina EditMode unit test yaz.
3. Performans olcumu yap (mobil hedef icin frame-time).

Basari kriteri:
- Regresyonlar testle yakalanir hale gelmeli.

## Faz 4 - Urunlestirme (Uzun Vade)

1. Ses/FX geri bildirimi tamamla.
2. Save/load ve ayar sistemi ekle.
3. Tutorial ve onboarding akisi ekle.
4. Build pipeline ve surum notu sureci olustur.

Basari kriteri:
- Prototipten demo/sunum kalitesine gecis.
