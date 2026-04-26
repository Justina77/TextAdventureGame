using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScriptedNpcDialogue : MonoBehaviour
{
    private enum Location
    {
        Library,
        RiverRoom,
        Storage,
        GreatHall
    }

    [Header("Language")]
    public bool isLatvian = false;

    [Header("UI")]
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI statusText;

    [Header("Story Scroll")]
    public ScrollRect storyScrollRect;
    public RectTransform storyContent;

    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceButtonTexts;

    private Location currentLocation = Location.Library;

    private bool hasStarted = false;
    private bool gameEnded = false;
    private bool playerWon = false;

    private bool librarySearched = false;
    private bool oldBookExamined = false;

    private bool hasTorch = false;
    private bool hasRustyKey = false;
    private bool hasLever = false;
    private bool hasDragonSword = false;
    private bool isSwordEquipped = false;

    private bool riverKeyDiscovered = false;

    private bool eastDoorOpened = false;
    private bool storageEntered = false;
    private bool leverDiscovered = false;

    private bool goldenDoorExamined = false;
    private bool goldenDoorOpened = false;
    private bool swordDiscovered = false;

    private bool dragonAwake = false;

    private void Start()
    {
        HideAllChoices();
    }

    public void SetLanguage(bool latvian)
    {
        isLatvian = latvian;

        if (hasStarted && !gameEnded)
        {
            RefreshCurrentScreen();
        }
    }

    public void BeginConversationIfNeeded()
    {
        if (hasStarted)
        {
            RefreshCurrentScreen();
            return;
        }

        hasStarted = true;
        ShowIntro();
    }

    private string T(string english, string latvian)
    {
        return isLatvian ? latvian : english;
    }

    private void ShowIntro()
    {
        currentLocation = Location.Library;

        ShowText(
            T(
                "You wake up in an old library. The air is heavy, dry, and full of dust. Tall bookshelves rise almost to the ceiling, but in the dim light it is difficult to see the details.\n\n" +
                "A magical book suddenly opens in front of you. Its pages turn by themselves, and glowing words appear on them:\n\n" +
                "“Greetings, traveler. I will guide you through this place. Your goal is to kill the dragon that has attacked towns and villages for years, burned homes, and stolen gold and treasures.\n\n" +
                "But the dragon cannot be defeated with an ordinary weapon. You must find a special sword — the dragon slayer sword.”\n\n" +
                "You can see several paths in the room: a large hole in the western wall, a rusty door to the east, and a golden door to the north.",

                "Tu atgūsti samaņu vecā bibliotēkā. Gaiss ir smags, sauss un putekļains. Augsti grāmatu plaukti sniedzas gandrīz līdz griestiem, bet puskrēslā ir grūti saskatīt detaļas.\n\n" +
                "Tavā priekšā pēkšņi atveras maģiska grāmata. Tās lapas pašas pāršķiras, un uz tām parādās mirdzošs teksts:\n\n" +
                "“Sveicināts, ceļotāj. Es tevi pavadīšu šajā vietā. Tavs mērķis ir nogalināt pūķi, kurš gadiem ilgi uzbruka pilsētām un ciemiem, dedzināja mājas un zaga zeltu un dārgumus.\n\n" +
                "Taču pūķi nevar uzvarēt ar parastu ieroci. Tev jāatrod īpašs zobens — pūķu slepkavas zobens.”\n\n" +
                "Telpā ir redzami vairāki ceļi: liels caurums rietumu sienā, sarūsējušas durvis austrumos un zelta durvis ziemeļos."
            ),
            new List<Choice>
            {
                new Choice(T("Look around", "Apskatīties apkārt"), LookAroundLibrary),
                new Choice(T("Approach the hole in the wall", "Pieiet pie cauruma sienā"), GoToRiverRoom),
                new Choice(T("Approach the rusty door", "Pieiet pie sarūsējušajām durvīm"), ApproachRustyDoor),
                new Choice(T("Approach the golden door", "Pieiet pie zelta durvīm"), ApproachGoldenDoor)
            }
        );
    }

    private void RefreshCurrentScreen()
    {
        if (gameEnded)
        {
            HideAllChoices();
            UpdateStatus();
            return;
        }

        switch (currentLocation)
        {
            case Location.Library:
                ShowLibraryActions(T(
                    "You are back in the Old Library.",
                    "Tu atkal esi Vecajā bibliotēkā."
                ));
                break;

            case Location.RiverRoom:
                ShowRiverRoomEntry();
                break;

            case Location.Storage:
                ShowStorageActions(T(
                    "You are in the Abandoned Storage Room.",
                    "Tu atrodies Pamestajā noliktavā."
                ));
                break;

            case Location.GreatHall:
                ShowGreatHallEntry();
                break;
        }
    }

    private void ShowLibraryActions(string text)
    {
        ShowText(text, BuildLibraryChoices());
    }

    private List<Choice> BuildLibraryChoices()
    {
        List<Choice> choices = new List<Choice>();

        if (!librarySearched)
        {
            choices.Add(new Choice(T("Look around", "Apskatīties apkārt"), LookAroundLibrary));
        }

        if (librarySearched && !hasTorch)
        {
            choices.Add(new Choice(T("Take the torch", "Paņemt lāpu"), TakeTorch));
        }

        if (librarySearched && !oldBookExamined)
        {
            choices.Add(new Choice(T("Examine the old book", "Apskatīt veco grāmatu"), ExamineOldBook));
        }

        choices.Add(new Choice(T("Go west through the hole in the wall", "Iet uz rietumiem caur caurumu sienā"), GoToRiverRoom));

        choices.Add(new Choice(
            eastDoorOpened
                ? T("Go to the Abandoned Storage Room", "Iet uz Pamesto noliktavu")
                : T("Approach the rusty door", "Pieiet pie sarūsējušajām durvīm"),
            ApproachRustyDoor
        ));

        choices.Add(new Choice(T("Approach the golden door", "Pieiet pie zelta durvīm"), ApproachGoldenDoor));

        return choices;
    }

    private void LookAroundLibrary()
    {
        librarySearched = true;

        ShowText(
            T(
                "You slowly look around the library.\n\n" +
                "The bookshelves are almost empty and covered with dust. The books that remain lie scattered on the floor. Many of them are rotten and fall apart at the slightest touch.\n\n" +
                "On one wall you notice an old torch. It may still be useful. Among the scattered books on the floor, you also notice one especially old book, almost completely ruined by time.\n\n" +
                "The magical book writes:\n\n" +
                "“It looks like this place was abandoned in a hurry. Someone took only what was truly necessary.”",

                "Tu lēnām apskati bibliotēku.\n\n" +
                "Grāmatu plaukti ir gandrīz tukši un pārklāti ar putekļiem. Grāmatas, kas vēl palikušas, mētājas uz grīdas. Daudzas no tām ir sapuvušas un drūp jau no mazākā pieskāriena.\n\n" +
                "Pie vienas sienas tu pamani vecu lāpu. Tā vēl varētu noderēt. Starp izmētātajām grāmatām uz grīdas tu pamani arī vienu īpaši vecu grāmatu, kuru laiks gandrīz pilnībā iznīcinājis.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Izskatās, ka šo vietu pameta steigā. Kāds paņēma tikai pašu nepieciešamāko.”"
            ),
            BuildLibraryChoices()
        );
    }

    private void TakeTorch()
    {
        hasTorch = true;

        ShowText(
            T(
                "You take the torch from the wall. It is old, but a weak orange flame still burns at its top.\n\n" +
                "The magical book trembles slightly in your hands.\n\n" +
                "“A good choice. In this place, there are rooms where your eyes will be useless without light.”",

                "Tu noņem lāpu no sienas. Tā ir veca, bet tās galā vēl deg vāja oranža liesma.\n\n" +
                "Maģiskā grāmata tavās rokās viegli nodreb.\n\n" +
                "“Laba izvēle. Šajā vietā ir telpas, kur bez gaismas acis būs bezspēcīgas.”"
            ),
            BuildLibraryChoices()
        );
    }

    private void ExamineOldBook()
    {
        oldBookExamined = true;

        ShowText(
            T(
                "You pick up the old book from the floor. Its cover almost crumbles in your hands, and the pages are darkened by time.\n\n" +
                "The title has been erased. Most of the text is unreadable, as if the ink dissolved in dampness.\n\n" +
                "But on one page you can still read a short message:\n\n" +
                "“RUN.”\n\n" +
                "The magical book remains silent for a moment. Then a new line appears on its page:\n\n" +
                "“Someone already tried to stop the dragon. Judging by this message, they were not ready.”",

                "Tu paceļ veco grāmatu no grīdas. Tās vāks gandrīz sabirst tavās rokās, un lapas ir satumsušas no laika.\n\n" +
                "Nosaukums ir izdzisis. Lielāko daļu teksta nav iespējams izlasīt, it kā tinte būtu izšķīdusi mitrumā.\n\n" +
                "Taču uz vienas lapas vēl var salasīt īsu uzrakstu:\n\n" +
                "“BĒDZ.”\n\n" +
                "Maģiskā grāmata kādu brīdi klusē. Tad uz tās lapas parādās jauna rinda:\n\n" +
                "“Kāds jau mēģināja apturēt pūķi. Spriežot pēc šī ieraksta, viņš nebija gatavs.”"
            ),
            BuildLibraryChoices()
        );
    }

    private void GoToRiverRoom()
    {
        currentLocation = Location.RiverRoom;
        ShowRiverRoomEntry();
    }

    private void ShowRiverRoomEntry()
    {
        if (!hasTorch)
        {
            ShowText(
                T(
                    "You pass through the hole in the wall and step into complete darkness.\n\n" +
                    "Somewhere ahead you hear the sound of water. A river seems to flow through this room, but you cannot see its banks or even the floor beneath your feet.\n\n" +
                    "The magical book glows faintly:\n\n" +
                    "“It is too dark here. Without a source of light, you cannot safely examine this room.”",

                    "Tu izej caur caurumu sienā un nonāc pilnīgā tumsā.\n\n" +
                    "Kaut kur priekšā ir dzirdama ūdens skaņa. Šķiet, cauri telpai tek upe, bet tu neredzi ne tās krastus, ne grīdu zem kājām.\n\n" +
                    "Maģiskā grāmata vāji iemirdzas:\n\n" +
                    "“Šeit ir pārāk tumšs. Bez gaismas avota tu nevarēsi droši apskatīt šo telpu.”"
                ),
                new List<Choice>
                {
                    new Choice(T("Try to move forward", "Mēģināt iet uz priekšu"), TryMoveInDark),
                    new Choice(T("Return to the library", "Atgriezties bibliotēkā"), ReturnToLibrary)
                }
            );

            return;
        }

        ShowText(
            T(
                "You raise the torch, and its light pushes back part of the darkness.\n\n" +
                "The room looks like a cave. The wet walls shine in the firelight, and an underground river flows through the middle of the room. The current looks fast and cold.\n\n" +
                "The magical book writes:\n\n" +
                "“Now you can examine this place more carefully.”",

                "Tu paceļ lāpu, un tās gaisma atstumj daļu tumsas.\n\n" +
                "Telpa izskatās kā ala. Mitrās sienas spīd uguns gaismā, un pa telpas vidu tek pazemes upe. Straume izskatās ātra un auksta.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Tagad tu vari uzmanīgāk apskatīt šo vietu.”"
            ),
            BuildRiverRoomChoices()
        );
    }

    private List<Choice> BuildRiverRoomChoices()
    {
        List<Choice> choices = new List<Choice>();

        if (!hasRustyKey)
        {
            if (riverKeyDiscovered)
            {
                choices.Add(new Choice(T("Take the rusty key", "Paņemt sarūsējušo atslēgu"), TakeRustyKey));
            }
            else
            {
                choices.Add(new Choice(T("Approach the river", "Pieiet pie upes"), ApproachRiver));
            }
        }

        choices.Add(new Choice(T("Examine the cave walls", "Apskatīt alas sienas"), ExamineCaveWalls));
        choices.Add(new Choice(T("Return to the library", "Atgriezties bibliotēkā"), ReturnToLibrary));

        return choices;
    }

    private void TryMoveInDark()
    {
        ShowText(
            T(
                "You take a few careful steps forward, but the darkness is too thick. The sound of water grows louder, and you realize that one wrong step could send you falling into the river.\n\n" +
                "“Do not risk it. Return and find a source of light.”",

                "Tu sper dažus piesardzīgus soļus uz priekšu, bet tumsa ir pārāk bieza. Ūdens skaņa kļūst skaļāka, un tu saproti, ka viens nepareizs solis var beigties ar kritienu upē.\n\n" +
                "“Neriskē. Atgriezies un atrodi gaismas avotu.”"
            ),
            new List<Choice>
            {
                new Choice(T("Return to the library", "Atgriezties bibliotēkā"), ReturnToLibrary)
            }
        );
    }

    private void ApproachRiver()
    {
        riverKeyDiscovered = true;

        ShowText(
            T(
                "You carefully approach the river. The torchlight reflects on the moving water.\n\n" +
                "Between the stones near the bank, something shines. You look closer and notice a small metal object.\n\n" +
                "The magical book writes:\n\n" +
                "“It looks like a rusty key. Perhaps it opens an old door.”",

                "Tu uzmanīgi pieej pie upes. Lāpas gaisma atspīd kustīgajā ūdenī.\n\n" +
                "Starp akmeņiem pie krasta kaut kas spīd. Tu ieskaties tuvāk un pamani nelielu metāla priekšmetu.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Izskatās pēc sarūsējušas atslēgas. Iespējams, tā atver kādas vecas durvis.”"
            ),
            BuildRiverRoomChoices()
        );
    }

    private void ExamineCaveWalls()
    {
        ShowText(
            T(
                "You examine the wet cave walls. There are old cracks in the stone and dark water stains.\n\n" +
                "You find nothing useful on the walls.\n\n" +
                "The magical book writes:\n\n" +
                "“Sometimes the important thing is not on the walls, but where it is harder to look.”",

                "Tu apskati mitrās alas sienas. Akmenī redzamas senas plaisas un tumši ūdens traipi.\n\n" +
                "Uz sienām tu neatrodi neko noderīgu.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Dažreiz svarīgais nav uz sienām, bet tur, kur skatīties ir grūtāk.”"
            ),
            BuildRiverRoomChoices()
        );
    }

    private void TakeRustyKey()
    {
        hasRustyKey = true;
        riverKeyDiscovered = false;

        ShowText(
            T(
                "You carefully lean toward the river and pull the rusty key out of the water. It is cold, heavy, and covered with dark stains.\n\n" +
                "The magical book writes:\n\n" +
                "“This key may open something old. There was a rusty door to the east in the library. Perhaps it belongs there.”",

                "Tu uzmanīgi noliecies pie upes un izvelc no ūdens sarūsējušo atslēgu. Tā ir auksta, smaga un pārklāta ar tumšiem traipiem.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Šī atslēga varētu atvērt kaut ko vecu. Bibliotēkā austrumos bija sarūsējušas durvis. Iespējams, tā der tām.”"
            ),
            BuildRiverRoomChoices()
        );
    }

    private void ReturnToLibrary()
    {
        currentLocation = Location.Library;

        ShowLibraryActions(
            T(
                "You return to the Old Library.\n\n" +
                "The dusty shelves, scattered books, rusty eastern door, and golden northern door are once again before you.",

                "Tu atgriezies Vecajā bibliotēkā.\n\n" +
                "Putekļainie plaukti, izmētātās grāmatas, sarūsējušās austrumu durvis un zelta durvis ziemeļos atkal ir tavā priekšā."
            )
        );
    }

    private void ApproachRustyDoor()
    {
        if (eastDoorOpened)
        {
            EnterStorage();
            return;
        }

        if (!hasRustyKey)
        {
            ShowText(
                T(
                    "You approach the eastern door. It is almost completely covered with rust, but the lock still holds firmly.\n\n" +
                    "You try to open it, but it does not move.\n\n" +
                    "The magical book writes:\n\n" +
                    "“This door is old, but the lock still works. You cannot open it without a key.”",

                    "Tu pieej pie austrumu durvīm. Tās gandrīz pilnībā klāj rūsa, bet slēdzene joprojām turas stingri.\n\n" +
                    "Tu mēģini tās atvērt, bet durvis nekustas.\n\n" +
                    "Maģiskā grāmata raksta:\n\n" +
                    "“Šīs durvis ir vecas, bet slēdzene joprojām darbojas. Bez atslēgas tās neatvērt.”"
                ),
                BuildLibraryChoices()
            );

            return;
        }

        eastDoorOpened = true;
        EnterStorage();
    }

    private void EnterStorage()
    {
        currentLocation = Location.Storage;
        storageEntered = true;

        ShowText(
            T(
                "You unlock the rusty door and enter the Abandoned Storage Room.\n\n" +
                "The room is filled with chests, boxes, and small caskets. Many of them are open, some are broken, and thick cobwebs hang in the corners.\n\n" +
                "The magical book writes:\n\n" +
                "“Something was stored here once. If the dragon took the gold, perhaps it left behind what it did not consider valuable.”",

                "Tu atslēdz sarūsējušās durvis un ieej Pamestajā noliktavā.\n\n" +
                "Telpa ir pilna ar lādēm, kastēm un mazām lādītēm. Daudzas no tām ir atvērtas, dažas salauztas, bet stūros karājas biezas zirnekļu tīklu kārtas.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Kādreiz šeit kaut kas tika glabāts. Ja pūķis paņēma zeltu, iespējams, viņš atstāja to, ko neuzskatīja par vērtīgu.”"
            ),
            BuildStorageChoices()
        );
    }

    private void ShowStorageActions(string text)
    {
        ShowText(text, BuildStorageChoices());
    }

    private List<Choice> BuildStorageChoices()
    {
        List<Choice> choices = new List<Choice>();

        if (!hasLever)
        {
            if (leverDiscovered)
            {
                choices.Add(new Choice(T("Take the lever", "Paņemt sviru"), TakeLever));
            }
            else
            {
                choices.Add(new Choice(T("Approach the chests", "Pieiet pie lādēm"), ApproachChests));
            }
        }
        else
        {
            choices.Add(new Choice(T("Examine the chests again", "Vēlreiz apskatīt lādes"), ApproachChests));
        }

        choices.Add(new Choice(T("Approach the small caskets", "Pieiet pie mazajām lādītēm"), ApproachBoxes));
        choices.Add(new Choice(T("Examine the cobwebs in the corner", "Apskatīt zirnekļu tīklus stūrī"), ExamineWeb));
        choices.Add(new Choice(T("Return to the library", "Atgriezties bibliotēkā"), ReturnToLibrary));

        return choices;
    }

    private void ApproachChests()
    {
        if (hasLever)
        {
            ShowText(
                T(
                    "You examine the chests again, but find nothing else useful.\n\n" +
                    "“The most important thing here is already with you. Now you should return to the golden door.”",

                    "Tu vēlreiz apskati lādes, bet neko citu noderīgu neatrodi.\n\n" +
                    "“Svarīgākā lieta šeit jau ir pie tevis. Tagad tev vajadzētu atgriezties pie zelta durvīm.”"
                ),
                BuildStorageChoices()
            );

            return;
        }

        leverDiscovered = true;

        ShowText(
            T(
                "You approach the chests and begin opening them one by one.\n\n" +
                "Most of them are empty. Some contain only dust, scraps of cloth, and marks left by stolen treasures.\n\n" +
                "Inside one large chest, you notice a long heavy rod. It looks too carefully shaped to be an ordinary piece of wood.\n\n" +
                "The magical book writes:\n\n" +
                "“This may be part of a mechanism. Perhaps this is the missing piece near the golden door.”",

                "Tu pieej pie lādēm un sāc tās atvērt vienu pēc otras.\n\n" +
                "Lielākā daļa ir tukšas. Dažās ir tikai putekļi, auduma gabali un pēdas no nozagtiem dārgumiem.\n\n" +
                "Vienā lielā lādē tu pamani garu, smagu stieni. Tas izskatās pārāk rūpīgi izgatavots, lai būtu parasts koka gabals.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Tas varētu būt mehānisma daļa. Iespējams, tieši šī detaļa trūkst pie zelta durvīm.”"
            ),
            BuildStorageChoices()
        );
    }

    private void TakeLever()
    {
        hasLever = true;
        leverDiscovered = false;

        ShowText(
            T(
                "You take the long heavy rod. In your hands it feels less like a piece of wood and more like part of an old mechanism.\n\n" +
                "The magical book writes:\n\n" +
                "“Return to the golden door. Now you can check whether this part fits the mechanism.”",

                "Tu paņem garo, smago stieni. Tavās rokās tas šķiet nevis kā parasts koka gabals, bet kā sena mehānisma daļa.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Atgriezies pie zelta durvīm. Tagad tu vari pārbaudīt, vai šī detaļa der mehānismam.”"
            ),
            BuildStorageChoices()
        );
    }

    private void ApproachBoxes()
    {
        ShowText(
            T(
                "You approach the small caskets. Some are decorated with faded patterns, but almost all of them are open and empty.\n\n" +
                "In one of them, spiders are moving. In another, you find only dust.\n\n" +
                "The magical book writes:\n\n" +
                "“The jewels are long gone. But not everything useful has to be hidden in a small casket.”",

                "Tu pieej pie mazajām lādītēm. Dažas ir rotātas ar izbalējušiem rakstiem, bet gandrīz visas ir atvērtas un tukšas.\n\n" +
                "Vienā no tām kustas zirnekļi. Citā tu atrodi tikai putekļus.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Dārglietas jau sen ir pazudušas. Taču ne viss noderīgais obligāti slēpjas mazā lādītē.”"
            ),
            BuildStorageChoices()
        );
    }

    private void ExamineWeb()
    {
        ShowText(
            T(
                "You carefully examine the cobwebs in the corner. Small splinters, dust, and dried insects are caught in them.\n\n" +
                "You find nothing useful.\n\n" +
                "The magical book writes:\n\n" +
                "“Spiders rarely guard what heroes truly need.”",

                "Tu uzmanīgi apskati zirnekļu tīklus stūrī. Tajos ieķērušās mazas šķembas, putekļi un izkaltuši kukaiņi.\n\n" +
                "Tu neatrodi neko noderīgu.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Zirnekļi reti sargā to, kas patiešām vajadzīgs varoņiem.”"
            ),
            BuildStorageChoices()
        );
    }

    private void ApproachGoldenDoor()
    {
        goldenDoorExamined = true;

        if (goldenDoorOpened)
        {
            ShowText(
                T(
                    "The golden door is already open. Beyond it lies the path to the Great Hall.\n\n" +
                    "The magical book writes:\n\n" +
                    "“The way to the dragon is open. Now you must decide whether you are ready to enter.”",

                    "Zelta durvis jau ir atvērtas. Aiz tām ved ceļš uz Lielo zāli.\n\n" +
                    "Maģiskā grāmata raksta:\n\n" +
                    "“Ceļš pie pūķa ir atvērts. Tagad tev jāizlemj, vai esi gatavs ieiet.”"
                ),
                BuildGoldenDoorChoices()
            );

            return;
        }

        ShowText(
            T(
                "You approach the golden door at the north side of the library.\n\n" +
                "It looks completely different from everything around it: clean, smooth, without rust or cracks. There is no handle and no lock.\n\n" +
                "Next to it stands a strange mechanism. There is an empty slot in it, as if a long part should be inserted there.\n\n" +
                "The magical book writes:\n\n" +
                "“This door does not open with a key. The mechanism is missing something.”",

                "Tu pieej pie zelta durvīm bibliotēkas ziemeļu pusē.\n\n" +
                "Tās izskatās pilnīgi citādi nekā viss apkārt: tīras, gludas, bez rūsas un plaisām. Tām nav ne roktura, ne slēdzenes.\n\n" +
                "Blakus tām atrodas dīvains mehānisms. Tajā ir tukša vieta, it kā tur būtu jāievieto gara detaļa.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Šīs durvis neatveras ar atslēgu. Mehānismam kaut kā trūkst.”"
            ),
            BuildGoldenDoorChoices()
        );
    }

    private List<Choice> BuildGoldenDoorChoices()
    {
        List<Choice> choices = new List<Choice>();

        if (!goldenDoorOpened && hasLever)
        {
            choices.Add(new Choice(T("Use the lever in the mechanism", "Izmantot sviru mehānismā"), UseLeverInMechanism));
        }

        if (goldenDoorOpened && !hasDragonSword && swordDiscovered)
        {
            choices.Add(new Choice(T("Take the dragon slayer sword", "Paņemt pūķu slepkavas zobenu"), TakeDragonSword));
        }

        if (hasDragonSword && !isSwordEquipped)
        {
            choices.Add(new Choice(T("Equip the sword", "Ekipēt zobenu"), EquipDragonSword));
        }

        if (goldenDoorOpened)
        {
            choices.Add(new Choice(T("Enter the Great Hall", "Iet Lielajā zālē"), EnterGreatHall));
        }

        choices.Add(new Choice(T("Return to the center of the library", "Atgriezties bibliotēkas centrā"), ReturnToLibrary));

        return choices;
    }

    private void UseLeverInMechanism()
    {
        hasLever = false;
        goldenDoorOpened = true;
        swordDiscovered = true;

        ShowText(
            T(
                "You insert the lever into the empty slot of the mechanism. It fits perfectly.\n\n" +
                "When you pull it down, a deep rumble echoes inside the wall. The golden door slowly opens.\n\n" +
                "Beyond the door, you see a passage leading to the Great Hall. Right before the entrance lies a sword, its blade glowing faintly in the darkness.\n\n" +
                "The magical book writes:\n\n" +
                "“There it is. The dragon slayer sword. Without it, entering the dragon's hall is pointless.”",

                "Tu ievieto sviru tukšajā mehānisma vietā. Tā der perfekti.\n\n" +
                "Kad tu to pavelc lejup, sienas iekšpusē atskan dziļa dunoņa. Zelta durvis lēnām atveras.\n\n" +
                "Aiz durvīm redzams ceļš uz Lielo zāli. Tieši pirms ieejas guļ zobens, kura asmens tumsā vāji mirdz.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Tur tas ir. Pūķu slepkavas zobens. Bez tā ieiet pie pūķa ir bezjēdzīgi.”"
            ),
            BuildGoldenDoorChoices()
        );
    }

    private void TakeDragonSword()
    {
        hasDragonSword = true;
        swordDiscovered = false;

        ShowText(
            T(
                "You take the dragon slayer sword. The blade feels heavy, but it fits perfectly in your hand.\n\n" +
                "For a moment, a pale line of light runs along the metal, and the air around you grows colder.\n\n" +
                "The magical book writes:\n\n" +
                "“Now you have the weapon that can kill the dragon. But carrying the sword is not enough — you must equip it before battle.”",

                "Tu paņem pūķu slepkavas zobenu. Asmens šķiet smags, bet rokā tas iegulst perfekti.\n\n" +
                "Uz mirkli pa metālu pārskrien bāla gaismas līnija, un gaiss ap tevi kļūst vēsāks.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Tagad tev ir ierocis, ar kuru var nogalināt pūķi. Bet ar zobena nēsāšanu nepietiek — pirms kaujas tas ir jāekipē.”"
            ),
            BuildGoldenDoorChoices()
        );
    }

    private void EquipDragonSword()
    {
        isSwordEquipped = true;

        ShowText(
            T(
                "You grip the sword tightly and raise the blade before you.\n\n" +
                "The sword seems to answer your resolve. Its light grows brighter.\n\n" +
                "The magical book writes:\n\n" +
                "“Now you are ready. Go to the Great Hall and finish what many before you could not.”",

                "Tu cieši satver zobena rokturi un paceļ asmeni sev priekšā.\n\n" +
                "Zobens it kā atbild tavai apņēmībai. Tā gaisma kļūst spožāka.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Tagad tu esi gatavs. Ej uz Lielo zāli un pabeidz to, ko daudzi pirms tevis nespēja.”"
            ),
            BuildGoldenDoorChoices()
        );
    }

    private void EnterGreatHall()
    {
        currentLocation = Location.GreatHall;
        ShowGreatHallEntry();
    }

    private void ShowGreatHallEntry()
    {
        ShowText(
            T(
                "You pass through the open golden door.\n\n" +
                "Beyond it, a wide passage leads into a vast chamber. The farther you go, the hotter the air becomes. The stone walls tremble with deep, heavy breathing.\n\n" +
                "At last, you enter the Great Hall.\n\n" +
                "The ceiling disappears into darkness, and the walls are covered with marks of fire and claws. In the center of the hall rises a mountain of gold, gemstones, goblets, crowns, and stolen treasures.\n\n" +
                "On top of that mountain sleeps a huge red dragon.\n\n" +
                "The magical book opens in your hands.\n\n" +
                "“This is the one. The dragon who burned towns and villages for years. Here, the paths end and the final choice begins.”",

                "Tu izej cauri atvērtajām zelta durvīm.\n\n" +
                "Aiz tām plats gaitenis ved milzīgā telpā. Jo tālāk tu ej, jo karstāks kļūst gaiss. Akmens sienas dreb no dziļas, smagas elpas.\n\n" +
                "Beidzot tu ieej Lielajā zālē.\n\n" +
                "Griesti pazūd tumsā, un sienas klāj uguns un nagu pēdas. Zāles centrā paceļas zelta, dārgakmeņu, kausu, kroņu un nozagtu dārgumu kalns.\n\n" +
                "Šī kalna virsotnē guļ milzīgs sarkans pūķis.\n\n" +
                "Maģiskā grāmata atveras tavās rokās.\n\n" +
                "“Tas ir viņš. Pūķis, kurš gadiem ilgi dedzināja pilsētas un ciemus. Šeit ceļi beidzas un sākas pēdējā izvēle.”"
            ),
            BuildGreatHallChoices()
        );
    }

    private List<Choice> BuildGreatHallChoices()
    {
        List<Choice> choices = new List<Choice>();

        if (hasDragonSword && !isSwordEquipped)
        {
            choices.Add(new Choice(T("Equip the sword", "Ekipēt zobenu"), EquipSwordInGreatHall));
        }

        choices.Add(new Choice(
            isSwordEquipped
                ? T("Attack the dragon with the sword", "Uzbrukt pūķim ar zobenu")
                : T("Attack the dragon", "Uzbrukt pūķim"),
            AttackDragon
        ));

        choices.Add(new Choice(T("Try to wake the dragon", "Mēģināt pamodināt pūķi"), WakeDragon));
        choices.Add(new Choice(T("Retreat to the Old Library", "Atkāpties uz Veco bibliotēku"), RetreatFromGreatHall));

        return choices;
    }

    private void EquipSwordInGreatHall()
    {
        isSwordEquipped = true;

        ShowText(
            T(
                "You draw the dragon slayer sword and grip its handle tightly.\n\n" +
                "The blade begins to shine with cold light. The dragon on the mountain of gold shifts, as if it senses the weapon made to destroy it.\n\n" +
                "The magical book writes:\n\n" +
                "“Now you are ready. Do not hesitate. The dragon has not fully risen yet.”",

                "Tu izvelc pūķu slepkavas zobenu un cieši satver tā rokturi.\n\n" +
                "Asmens sāk mirdzēt aukstā gaismā. Pūķis uz zelta kalna sakustas, it kā sajustu ieroci, kas radīts viņa iznīcināšanai.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Tagad tu esi gatavs. Nevilcinies. Pūķis vēl nav pilnībā piecēlies.”"
            ),
            BuildGreatHallChoices()
        );
    }

    private void WakeDragon()
    {
        dragonAwake = true;

        ShowText(
            T(
                "You take a step closer to the mountain of gold.\n\n" +
                "A coin rings beneath your foot.\n\n" +
                "The sound is loud enough.\n\n" +
                "The dragon slowly opens one eye. Its pupil looks like a burning slit in the dark. The air grows hotter, and the gold beneath it begins to tremble.\n\n" +
                "The magical book writes:\n\n" +
                "“It is waking. If you intend to fight, it is too late to doubt yourself.”",

                "Tu sper soli tuvāk zelta kalnam.\n\n" +
                "Zem tavas kājas nošķind monēta.\n\n" +
                "Ar šo skaņu pietiek.\n\n" +
                "Pūķis lēnām atver vienu aci. Tā zīlīte izskatās kā degoša sprauga tumsā. Gaiss kļūst karstāks, un zelts zem viņa sāk drebēt.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Viņš mostas. Ja tu grasies cīnīties, šaubām vairs nav laika.”"
            ),
            BuildDragonAwakeChoices()
        );
    }

    private List<Choice> BuildDragonAwakeChoices()
    {
        List<Choice> choices = new List<Choice>();

        if (hasDragonSword && !isSwordEquipped)
        {
            choices.Add(new Choice(T("Equip the sword", "Ekipēt zobenu"), EquipSwordInGreatHall));
        }

        choices.Add(new Choice(
            isSwordEquipped
                ? T("Attack the dragon with the sword", "Uzbrukt pūķim ar zobenu")
                : T("Attack the dragon", "Uzbrukt pūķim"),
            AttackDragon
        ));

        if (!isSwordEquipped)
        {
            choices.Add(new Choice(T("Try to retreat", "Mēģināt atkāpties"), LoseRetreatTooLate));
        }
        else
        {
            choices.Add(new Choice(T("Retreat", "Atkāpties"), RetreatFromGreatHall));
        }

        return choices;
    }

    private void AttackDragon()
    {
        if (isSwordEquipped)
        {
            WinGame();
            return;
        }

        if (hasDragonSword)
        {
            LoseSwordNotEquipped();
            return;
        }

        LoseNoSword();
    }

    private void RetreatFromGreatHall()
    {
        if (dragonAwake && !isSwordEquipped)
        {
            LoseRetreatTooLate();
            return;
        }

        currentLocation = Location.Library;
        dragonAwake = false;

        ShowLibraryActions(
            T(
                "You slowly step back, trying not to touch the gold or make another sound.\n\n" +
                "The dragon continues to sleep on top of its treasure, though its breathing becomes heavier.\n\n" +
                "You return through the golden door to the Old Library.\n\n" +
                "The magical book writes:\n\n" +
                "“You can still prepare. Return only when the sword is in your hands and you are ready to use it.”",

                "Tu lēnām atkāpies, cenšoties neaizskart zeltu un neizdot vēl kādu skaņu.\n\n" +
                "Pūķis turpina gulēt uz saviem dārgumiem, lai gan viņa elpa kļūst smagāka.\n\n" +
                "Tu atgriezies caur zelta durvīm Vecajā bibliotēkā.\n\n" +
                "Maģiskā grāmata raksta:\n\n" +
                "“Tu vēl vari sagatavoties. Atgriezies tikai tad, kad zobens būs tavās rokās un tu būsi gatavs to izmantot.”"
            )
        );
    }

    private void WinGame()
    {
        playerWon = true;
        gameEnded = true;

        ShowText(
            T(
                "You raise the dragon slayer sword and rush forward.\n\n" +
                "The dragon opens its eyes fully. Fire gathers in its mouth, and its enormous wings lift ash, gold, and dust into the air.\n\n" +
                "It roars so loudly that the walls of the Great Hall tremble.\n\n" +
                "But the sword in your hands shines brighter and brighter.\n\n" +
                "You avoid the first stream of fire and climb the slope of the golden mountain. Coins slide down, gemstones roll beneath your feet, but you keep moving.\n\n" +
                "The dragon raises a claw to crush you.\n\n" +
                "At that moment, you strike.\n\n" +
                "The blade pierces the red scales that no ordinary weapon could break. Light bursts from the wound, and the dragon gives one final terrible roar.\n\n" +
                "The fire in its mouth dies.\n\n" +
                "Its huge body falls onto the mountain of gold and moves no more.\n\n" +
                "The magical book opens by itself. Its pages shine with soft light.\n\n" +
                "“The dragon is defeated. The towns and villages that lived in fear are free. You found the sword, prepared yourself, and did what many before you could not.\n\n" +
                "Your journey is complete.”",

                "Tu paceļ pūķu slepkavas zobenu un metas uz priekšu.\n\n" +
                "Pūķis pilnībā atver acis. Viņa mutē sakrājas uguns, un milzīgie spārni paceļ gaisā pelnus, zeltu un putekļus.\n\n" +
                "Viņš ierēcas tik skaļi, ka Lielās zāles sienas nodreb.\n\n" +
                "Taču zobens tavās rokās mirdz arvien spožāk.\n\n" +
                "Tu izvairies no pirmās liesmu straumes un rāpies augšup pa zelta kalnu. Monētas birst lejup, dārgakmeņi ripo zem kājām, bet tu turpini kustēties.\n\n" +
                "Pūķis paceļ nagu, lai tevi sadragātu.\n\n" +
                "Tajā brīdī tu sit.\n\n" +
                "Asmens caurdur sarkanās zvīņas, kuras nespēja pāršķelt neviens parasts ierocis. No brūces izlaužas gaisma, un pūķis izdveš pēdējo šausminošo rēcienu.\n\n" +
                "Uguns viņa mutē izdziest.\n\n" +
                "Milzīgais ķermenis nokrīt uz zelta kalna un vairs nekustas.\n\n" +
                "Maģiskā grāmata pati atveras. Tās lapas iemirdzas maigā gaismā.\n\n" +
                "“Pūķis ir uzvarēts. Pilsētas un ciemi, kas dzīvoja bailēs, tagad ir brīvi. Tu atradi zobenu, sagatavojies un paveici to, ko daudzi pirms tevis nespēja.\n\n" +
                "Tavs ceļojums ir pabeigts.”"
            ),
            new List<Choice>()
        );
    }

    private void LoseNoSword()
    {
        playerWon = false;
        gameEnded = true;

        ShowText(
            T(
                "You rush at the dragon without a weapon capable of piercing its scales.\n\n" +
                "The dragon opens its eyes.\n\n" +
                "For a moment, the entire hall seems to freeze. Then the air becomes unbearably hot.\n\n" +
                "The dragon raises its head, and a stream of fire bursts from its mouth.\n\n" +
                "The magical book flashes with light, as if trying to warn you one last time.\n\n" +
                "“Without the dragon slayer sword, it cannot be defeated.”\n\n" +
                "The flames cover everything.\n\n" +
                "Your journey ends here.",

                "Tu meties virsū pūķim bez ieroča, kas spētu caurdurt viņa zvīņas.\n\n" +
                "Pūķis atver acis.\n\n" +
                "Uz mirkli šķiet, ka visa zāle sastingst. Tad gaiss kļūst neciešami karsts.\n\n" +
                "Pūķis paceļ galvu, un no viņa mutes izlaužas uguns straume.\n\n" +
                "Maģiskā grāmata uzplaiksnī gaismā, it kā mēģinātu tevi pēdējo reizi brīdināt.\n\n" +
                "“Bez pūķu slepkavas zobena viņu nevar uzvarēt.”\n\n" +
                "Liesmas pārklāj visu apkārt.\n\n" +
                "Tavs ceļojums šeit beidzas."
            ),
            new List<Choice>()
        );
    }

    private void LoseSwordNotEquipped()
    {
        playerWon = false;
        gameEnded = true;

        ShowText(
            T(
                "You rush at the dragon, but too late you understand your mistake.\n\n" +
                "You have the dragon slayer sword, but it is not in your hands.\n\n" +
                "The dragon wakes instantly. Its eyes burn red, and a strike of its tail shatters part of the golden mountain.\n\n" +
                "You try to reach for the sword, but there is no time.\n\n" +
                "The magical book writes its final line:\n\n" +
                "“A weapon that is not ready for battle cannot save its owner.”\n\n" +
                "The dragon releases its fire.\n\n" +
                "Your journey ends here.",

                "Tu meties virsū pūķim, bet pārāk vēlu saproti savu kļūdu.\n\n" +
                "Pūķu slepkavas zobens tev ir, bet tas nav tavās rokās.\n\n" +
                "Pūķis pamostas acumirklī. Viņa acis iedegas sarkanā gaismā, un astes trieciens sadragā daļu zelta kalna.\n\n" +
                "Tu mēģini sniegties pēc zobena, bet laika vairs nav.\n\n" +
                "Maģiskā grāmata uzraksta pēdējo rindu:\n\n" +
                "“Ierocis, kas nav gatavs kaujai, nevar izglābt savu īpašnieku.”\n\n" +
                "Pūķis izlaiž uguni.\n\n" +
                "Tavs ceļojums šeit beidzas."
            ),
            new List<Choice>()
        );
    }

    private void LoseRetreatTooLate()
    {
        playerWon = false;
        gameEnded = true;

        ShowText(
            T(
                "You take one step back, then another.\n\n" +
                "But the dragon is already awake.\n\n" +
                "Its head turns toward you, and its enormous eyes follow every movement. You try to run to the exit, but the hall is too large and the dragon is too fast.\n\n" +
                "One sweep of its wing knocks you to the ground.\n\n" +
                "The magical book falls beside you and opens on its final page.\n\n" +
                "“Sometimes retreat can save you. But only if it begins in time.”\n\n" +
                "Then the hall fills with fire.\n\n" +
                "Your journey ends here.",

                "Tu sper vienu soli atpakaļ, tad vēl vienu.\n\n" +
                "Bet pūķis jau ir pamodies.\n\n" +
                "Viņa galva pagriežas pret tevi, un milzīgās acis seko katrai kustībai. Tu mēģini aizskriet līdz izejai, bet zāle ir pārāk liela, un pūķis pārāk ātrs.\n\n" +
                "Viens spārna vēziens nogāž tevi zemē.\n\n" +
                "Maģiskā grāmata nokrīt tev blakus un atveras pēdējā lapā.\n\n" +
                "“Dažreiz atkāpšanās var glābt. Bet tikai tad, ja tā sākas laikus.”\n\n" +
                "Pēc tam zāli piepilda uguns.\n\n" +
                "Tavs ceļojums šeit beidzas."
            ),
            new List<Choice>()
        );
    }

    private void ShowText(string text, List<Choice> choices)
    {
        if (storyText != null)
        {
            storyText.text = text;
        }

        ResizeStoryContent();
        StartCoroutine(ScrollStoryToTop());

        UpdateStatus();
        ShowChoices(choices);
    }

    private void ResizeStoryContent()
    {
        if (storyText == null || storyContent == null)
        {
            return;
        }

        storyText.ForceMeshUpdate();

        float preferredHeight = storyText.preferredHeight + 80f;
        float minHeight = 400f;

        storyContent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            Mathf.Max(preferredHeight, minHeight)
        );
    }

    private IEnumerator ScrollStoryToTop()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (storyScrollRect != null)
        {
            storyScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void ShowChoices(List<Choice> choices)
    {
        HideAllChoices();

        if (gameEnded)
        {
            return;
        }

        int count = Mathf.Min(choices.Count, choiceButtons.Length, choiceButtonTexts.Length);

        for (int i = 0; i < count; i++)
        {
            int index = i;

            choiceButtons[i].gameObject.SetActive(true);
            choiceButtonTexts[i].text = choices[i].text;

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() =>
            {
                choices[index].action.Invoke();
            });
        }
    }

    private void HideAllChoices()
    {
        if (choiceButtons == null)
        {
            return;
        }

        foreach (Button button in choiceButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateStatus()
    {
        if (statusText == null)
        {
            return;
        }

        statusText.text =
            $"{T("Location", "Lokācija")}: {GetLocationName()}\n" +
            $"{T("Inventory", "Inventārs")}: {GetInventoryText()}\n" +
            $"{T("Equipped", "Ekipēts")}: {GetEquippedText()}\n" +
            $"{T("Discovered", "Atklāts")}: {GetDiscoveredText()}" +
            (gameEnded
                ? $"\n{T("Result", "Rezultāts")}: {(playerWon ? T("victory", "uzvara") : T("defeat", "sakāve"))}"
                : "");
    }

    private string GetLocationName()
    {
        switch (currentLocation)
        {
            case Location.Library:
                return T("Old Library", "Vecā bibliotēka");

            case Location.RiverRoom:
                return T("River Room", "Telpa ar upi");

            case Location.Storage:
                return T("Abandoned Storage Room", "Pamestā noliktava");

            case Location.GreatHall:
                return T("Great Hall", "Lielā zāle");

            default:
                return T("Unknown", "Nezināms");
        }
    }

    private string GetInventoryText()
    {
        List<string> items = new List<string>();

        if (hasTorch)
        {
            items.Add(T("torch", "lāpa"));
        }

        if (hasRustyKey)
        {
            items.Add(T("rusty key", "sarūsējusī atslēga"));
        }

        if (hasLever)
        {
            items.Add(T("lever", "svira"));
        }

        if (hasDragonSword)
        {
            items.Add(T("dragon slayer sword", "pūķu slepkavas zobens"));
        }

        return items.Count == 0 ? T("empty", "tukšs") : string.Join(", ", items);
    }

    private string GetEquippedText()
    {
        return isSwordEquipped
            ? T("dragon slayer sword", "pūķu slepkavas zobens")
            : T("nothing", "nekas");
    }

    private string GetDiscoveredText()
    {
        List<string> discovered = new List<string>();

        switch (currentLocation)
        {
            case Location.Library:
                if (librarySearched && !hasTorch)
                {
                    discovered.Add(T("torch", "lāpa"));
                }

                if (librarySearched && !oldBookExamined)
                {
                    discovered.Add(T("old book", "veca grāmata"));
                }

                if (oldBookExamined)
                {
                    discovered.Add(T("examined old book", "apskatīta vecā grāmata"));
                }

                if (goldenDoorExamined)
                {
                    discovered.Add(T("mechanism near the golden door", "mehānisms pie zelta durvīm"));
                }

                if (goldenDoorOpened)
                {
                    discovered.Add(T("open passage to the Great Hall", "atvērts ceļš uz Lielo zāli"));
                }

                if (swordDiscovered && !hasDragonSword)
                {
                    discovered.Add(T("dragon slayer sword", "pūķu slepkavas zobens"));
                }

                break;

            case Location.RiverRoom:
                if (!hasTorch)
                {
                    discovered.Add(T("sound of a river", "upes skaņa"));
                }
                else
                {
                    discovered.Add(T("river", "upe"));
                }

                if (riverKeyDiscovered && !hasRustyKey)
                {
                    discovered.Add(T("rusty key in the river", "sarūsējusī atslēga upē"));
                }

                break;

            case Location.Storage:
                if (storageEntered)
                {
                    discovered.Add(T("chests", "lādes"));
                    discovered.Add(T("small caskets", "mazās lādītes"));
                }

                if (leverDiscovered && !hasLever)
                {
                    discovered.Add(T("lever", "svira"));
                }

                break;

            case Location.GreatHall:
                discovered.Add(dragonAwake
                    ? T("awakened dragon", "pamodies pūķis")
                    : T("sleeping dragon", "guļošs pūķis"));

                discovered.Add(T("mountain of gold and treasures", "zelta un dārgumu kalns"));
                break;
        }

        return discovered.Count == 0 ? T("nothing", "nekas") : string.Join(", ", discovered);
    }

    private class Choice
    {
        public string text;
        public Action action;

        public Choice(string text, Action action)
        {
            this.text = text;
            this.action = action;
        }
    }
}