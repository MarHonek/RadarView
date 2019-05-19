# Spuštění aplikace
Aplikaci spouští soubor **RadarView.exe** v adresáři aplikace.

# Ovládání
|       Ovládání myši        |      Akce     |
| ------------- | ------------- |
| Kliknutím levého tlačítka myši na popisek cíle. | Označení cíle. Přenutí základního/rozšířeného zobrazení informací o letadle.  |
| Držením levého tlačítka myši na popisku cíle + pohybem myši. | Posunutí popisku cíle.  |
| Držením levého tlačítka myši (mimo popisek) + pohybem myši.  | Posunutí zobrazení.  |
| Pohybem kolečka myši. | Přiblížení/oddálení zobrazení (zoom). |
| Držením pravého tlačítka myši + pohybem myši. | Vytvoření měřící linie. |
| Kliknutím pravého tlačítka myši. | Zrušení měřící linie. |


# Zvýrazňování letadel a vzdušných prostorů pomocí protokolu UDP
Pro zvýraznění vzdušných prostorů a letadel je nutné odeslat zprávu na IP adresu počítače, na kterém běží tato aplikace. \
Síťová komunikace musí probíhat prostřednictvím protokolu UDP na síťovém portu **11000**.

Odesílaná zpráva musí dodržovat následující formát:

> {"activeAirspaces":["_název prostoru_"],"highlightedAircrafts":{"_identifikátor letadla_":"_typ zvýraznění letadla_"}}

#### Název prostoru
Názvy vzdušných prostorů lze najít v souboru **airports.csv**, ke kterému vede cesta *[Adresář aplikace]\Content\AviationData\openaip_airspace_czech_republic_cz.aip*

#### Identifikátor letadla
Pro zvýraznění je možné následující identifikátory letadla:
1. Adresu zařízení odesílající informace o letadle. Tato adresa může být:
    * Adresa odpovídače v módu S.
    * Adresa zařízení OGN Tracker.
    * Adresa zařízení FLARM.
2. Registrační značku letadla.
3. Číslo letu (IATA)
4. Volací znak letadla. 


                
#### Typy zvýraznění letadel
> `Alert`, 
> `Notice1`, 
> `Notice2`.

Identifikátor letadla, typ zvýraznění letadla i název vzdušného prostoru se vyhodnocuje case-insensitive. 

Příklady:
UDP --> 127.0.0.1:1100
> `{"activeAirspaces":["CTR turany"], "highlightedAircrafts":{"KLM1978":"Notice1"}}` \
> `{"activeAirspaces":[], "highlightedAircrafts":{}}` -- vypne všechna zvýraznění (letadel i prostorů) 


# Replay vzdušné situace
Informace o letadlech jsou standardně ukládány. \
Cesta k uloženým informacím: *[Adresář aplikace]\\DataSource* \
V adresáři *DataSource* se nachází adresáře pro jednotlivé datové zdroje, ze kterých jsou informace přijímány. \
Tyto adresáře obsahují data rozčleněna podle datumu a času jejich uložení.

Data v rámci jedné hodiny jsou uložený do souboru s názvem ve formátu *HH.txt*. \
Soubory v rámci jednoho dne jsou uloženy v adresáři s názvem ve formátu *yyyy-MM-dd*.

*yyyy* = rok, \
*MM* = měsíc, \
*dd* = den, \
*HH* = hodina (UTC).

#### Postup přehrání
1. Zkopírovat vybrané řádky ze souboru obsahující uložená data.
2. V adresáři s cestou:  *[Adresář aplikace]\DataSource\\[Název Datového zdroje]*   vytvořit nový soubor **ReplayX.txt**, kde X je číslo z intervalu [0;9].
3. Do souboru vložit zkopírované řádky. 
4. Spustit aplikaci s argumenty:
    * **-r** (spustí replay)
    * **-gX** (načte data ze souboru s číslem X, který je uložený v adresáři *OpenGliderNetwork*)
    * **-sX** (načte data ze souboru s číslem X, který je uložený v adresáři *OpenSkyNetwork*)


**Příklad přehrání informací z datového zdroje Open Glider Network:**
1. Do adresáře *[Adresář aplikace]\DataSource\OpenGliderNetwork\Replay0.txt* jsou vložena data.
2. Aplikace spuštěna s argumenty: *-r -g0*.


### Import mapových podkladů:
1. vložit obrázky (mapy) do složky Content ve tvaru: **NázevMapy_IcaoLetiště_ZoomLevel** \
    -přípona musí být **.png** nebo **.jpg**
2. V konfiguračním souboru upravit JSON záznam s klíčem  **'MapConfig'**. \
    -příklad JSON řetezce: > `{"mapLayers":[{"name":"Základní mapa","imageName":"BasicMap","isVisible":false,"boundingBoxes":{"9":"51.85402 12.43535 46.47334 20.675","11":"49.90481 15.52526 48.5598 17.58519","13":"49.40472 16.29773 49.06847 16.81272"}},{"name":"VFR","imageName":"VfrMap","isVisible":false,"boundingBoxes":{"9":"50.287530 14.770606 47.998561 18.279964"}}]} `

-**name**: název typu mapy, který se zobrazí v tlačítku na zobrazení/skrytí mapy. \
-**imageName**: název mapy, který je součástí názvu souboru. \
-**boudingBoxes**: souřadnice pro úrovně zoomu [sever západ jih východ].\
-**visibility**: true nebo false. Příznak, zda má být mapa zobrazena při spuštění aplikace.

### Konfigurační soubor.
Nastavení aplikace je uloženo v konfiguračním souboru **user.config**
Cesta ke konfiguračnímu souboru je: *%AppData%\RadarView\Radarview_Url_[hash]\\[verze aplikace]\user.config* uživatelském adresáři systémového disku.
Příklad:
> C:\Users\Martin\AppData\Local\RadarView\RadarView.exe_Url_uiwdpypmvjddtfp21gxdnwlx4nqs02gj\1.3.0.0\user.config

Další informace o návrhu a implementaci lze nalézt v bakalářské práci, v rámci které byla tato aplikace vytvořena.
Bakalářská práce je dostupná z: *brzy*.

