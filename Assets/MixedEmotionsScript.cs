using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Math = ExMath;

public class MixedEmotionsScript : MonoBehaviour {

  public KMBombInfo Bomb;
  public KMBombModule Module;
  public KMBombInfo BombInfo;
  public KMAudio Audio;
  public KMSelectable[] Buttons;
  public TextMesh[] DisplayTexts;
  public Sprite Happy;
  public Sprite Sad;
  public Sprite Empty;
  public SpriteRenderer spriteRenderer;
  public Renderer bgRenderer;
  public Material solvedMaterial;
  public Renderer borderRenderer;
  public Material normalBorder;
  public Material GoCrazyBorder;

   private int lovesmenotwonder = 0;
   public string whatSheSaid;
   private int selectedMessageIndex;
   public int messageIndex;
   bool isTyping = false;
   bool QuickPass = false;
   int love = 0;
   int buttonAnswer = 0;
   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;
   private bool PressMePressed;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      GetComponent<KMBombModule>().OnActivate += Activate;
      /*
      foreach (KMSelectable object in keypad) {
          object.OnInteract += delegate () { keypadPress(object); return false; };
      }
      */

      //Loves Me
      Buttons[0].OnInteract += delegate () { submitLovesMe(); return false; };

      //Loves Me Not
      Buttons[1].OnInteract += delegate () { submitLovesMeNot(); return false; };

      //HeartScreen/PressMe
      Buttons[2].OnInteract += delegate () { beginModule(); return false; };

      //Interaction PUNCHES!!!
      Buttons[0].AddInteractionPunch();
      Buttons[1].AddInteractionPunch();

   }

   //The agony...
   //All arrays start at 0!!
   //10 strings, so array goes to 9.
   string[] messages = new string[]
     {
         "we never call anymore.\ni really miss u. :(",
         "i want this to work out\nbut it's so hard\nwhen we can't even\nconversate like we should",
         "i'm not sure if we're on\nthe same page anymore...",
         "i don't want\nto lose u",
         "i still love you, but things\nhave been very\ncomplicated lately",
         "i don't think i was ready\nfor this, is all",
         "sometimes i wonder if\nwe're better off as\nfriends.",
         "i know I'm special to you,\nbut something just seems\nsuper wrong about this.",
         "i feel like i'm\ncausing you pain because\ni can't always be there",
         "do you think we rushed\ninto this?",
     };

   void DetermineString() {
       StartCoroutine(TypeCharacters());
   }

   public float typingSpeed = 0.1f;
   private IEnumerator TypeCharacters()
   {
       DisplayTexts[0].text = "";
       selectedMessageIndex = UnityEngine.Random.Range(0, messages.Length);
       string gfMessage = messages[selectedMessageIndex];
       whatSheSaid = gfMessage.Replace("\n", " ");
       isTyping = true;
       for (int i = 0; i < gfMessage.Length; i++)
       {
         DisplayTexts[0].text += gfMessage[i];
           if (i % 26 == 0) // Play sound every 5 characters (adjust as needed)
           {
               Audio.PlaySoundAtTransform("Typing", DisplayTexts[3].transform);
           }
           if (ModuleSolved || QuickPass)
           {
                yield break; // Exit the coroutine if module is solved
          }
           yield return new WaitForSeconds(typingSpeed);
       }
   }

   private IEnumerator UndoQuickPass()
   {
       yield return new WaitForSeconds(3f);
       QuickPass = false;
   }

   void submitAnswer(int answer)
   {
       if (answer == buttonAnswer)
       {
           if(answer==1)
           {
             lovesmenotwonder = answer;
           }
           else{
             lovesmenotwonder=0;
           }
           Audio.PlaySoundAtTransform("Solve", DisplayTexts[0].transform);
           Solve();
           SheLovesMe();
           Debug.LogFormat("[Mixed Emotions " + "#" + ModuleId + "] Solved! The answer was button " + answer + ".", ModuleId);
       }
       else
       {
           PressMePressed = false;
           QuickPass = true;
           StartCoroutine(UndoQuickPass());
           StartCoroutine(FlashFaultyColor());
           Audio.PlaySoundAtTransform("Strike", DisplayTexts[0].transform);
           Strike();
           Debug.LogFormat("[Mixed Emotions " + "#" + ModuleId + "] Strike! An incorrect answer of button " + answer + " was submitted.", ModuleId);
           spriteRenderer.sprite = Empty;
           //Best way to restart it
       }
   }

   void submitLovesMe() {
  if(ModuleSolved) {
    //GET A JOB!!!
  }
  else {
    submitAnswer(0);
    Audio.PlaySoundAtTransform("Beep", Buttons[0].transform);
  }
}

void submitLovesMeNot() {
if(ModuleSolved) {
 //GET A JOB!!!
}
else {
  submitAnswer(1);
  Audio.PlaySoundAtTransform("Beep", Buttons[1].transform);
}
}

void SecondTry(){
  DetermineString();
  DetermineLove();
  PressMePressed = true;
}

void beginModule() {
    if(!QuickPass){
    if(!PressMePressed)
    {
     DisplayTexts[3].text = ""; //Removes PressMeText
     int spriteIndex = UnityEngine.Random.Range(0, 2);
     if (spriteIndex == 0)
{
    spriteRenderer.sprite = Happy;
    love = 1; //Love is 1, because one is happy even when alone.... not really...
}
else
{
    spriteRenderer.sprite = Sad;
    love = 0;
}
SecondTry();
}
else{
  //Do nothing. Don't want to press the button more than once.
}
   }
}

   void DetermineLove()
   {
      int messageIndex = selectedMessageIndex;
       if (!ModuleSolved)
       {
           switch (messageIndex)
           {
               case 9:
                   buttonAnswer = love == 1 ? 1 : 0;
                   break;
               case 8:
                   buttonAnswer = love == 0 ? 0 : 1;
                   break;
               case 5:
               case 7:
                   buttonAnswer = love == 1 ? 0 : 1;
                   break;
               case 6:
               case 1:
                   buttonAnswer = love == 0 ? 1 : 0;
                   break;
              case 3:
               case 4:
               case 2:
               case 0:
                   buttonAnswer = love == 0 ? 0 : 1;
                   break;
               default:
                   break;
           }
           Debug.LogFormat("[Mixed Emotions " + "#" + ModuleId + "] The message reads: '" + whatSheSaid + "'.", ModuleId);
           Debug.LogFormat("[Mixed Emotions " + "#" + ModuleId + "] The message index number is: " + selectedMessageIndex, ModuleId);
           if(love==0)
           {
             Debug.LogFormat("[Mixed Emotions " + "#" + ModuleId + "] The heart is cracked.", ModuleId);
           }
           else{
            Debug.LogFormat("[Mixed Emotions " + "#" + ModuleId + "] The heart is healthy.", ModuleId);
           }
           Debug.LogFormat("[Mixed Emotions " + "#" + ModuleId + "] The button that needs to be pressed is: " + buttonAnswer, ModuleId);
       }
       else
       {
           // Module solved
       }
   }

   void OnDestroy () { //Shit you need to do when the bomb ends

   }

   void Activate () { //Shit that should happen when the bomb arrives (factory)/Lights turn on
   }

   void Start () { //Shit
   }

   void Update () { //Shit that happens at any point after initialization
   }

   void Solve () {
      GetComponent<KMBombModule>().HandlePass();
   }

   void Strike () {
      GetComponent<KMBombModule>().HandleStrike();
   }


   //Text flashes on win
   private float flashDuration = 0.2f;
   private float totalDuration = 25f;
   private IEnumerator FlashColor()
   {
       float elapsedTime = 0f;
       while (elapsedTime < totalDuration)
       {
           DisplayTexts[0].color = Color.white;
           yield return new WaitForSeconds(flashDuration);
           DisplayTexts[0].color = Color.green;
           yield return new WaitForSeconds(flashDuration);
           elapsedTime += 2 * flashDuration;
       }
       // Ensure the final color is consistent
       DisplayTexts[0].color = Color.green;
   }

   private float flashFaultDuration = 0.05f;
   private float totalFaultDuration = 1f;
   private IEnumerator FlashFaultyColor()
   {
       float elapsedTime2 = 0f;
       while (elapsedTime2 < totalFaultDuration)
       {
           DisplayTexts[0].color = Color.white;
           spriteRenderer.sprite = Empty;
           yield return new WaitForSeconds(flashFaultDuration);
           DisplayTexts[0].color = Color.red;
           spriteRenderer.sprite = Sad;
           yield return new WaitForSeconds(flashFaultDuration);
           elapsedTime2 += 2 * flashFaultDuration;
       }
       // Ensure the final color is consistent
       DisplayTexts[0].color = Color.white;
       DisplayTexts[0].text = "";
       spriteRenderer.sprite = Empty;
       DisplayTexts[3].text = "PRESS\nME";
   }

   private IEnumerator GoCrazy()
   {
       float elapsedTime = 0f;
       while (elapsedTime < totalDuration)
       {
           borderRenderer.material = normalBorder;
           yield return new WaitForSeconds(flashDuration);
           borderRenderer.material = GoCrazyBorder;
           yield return new WaitForSeconds(flashDuration);
           elapsedTime += 2 * flashDuration;
       }
       // Ensure the final color is consistent
       borderRenderer.material = normalBorder;
   }

   private IEnumerator GoStupidGoCrazy()
{
    yield return new WaitForSeconds(14.6f);
    StartCoroutine(GoCrazy());
}

private IEnumerator ResetTextOnWin()
{
 yield return new WaitForSeconds(0.1f);
 DisplayTexts[0].text = "SHE LOVES\nME!";
}

private IEnumerator FixLovesMeText()
{
 yield return new WaitForSeconds(0.1f);
 DisplayTexts[0].text = "SHE LOVES\nME NOT!";
}


  void SheLovesMe(){
    ModuleSolved = true;
    DisplayTexts[0].text = "SHE LOVES\nME!";
    DisplayTexts[0].fontSize = 300;
    bgRenderer.material = solvedMaterial;
    spriteRenderer.sprite = Happy;
    StartCoroutine(FlashColor());
    StartCoroutine(GoStupidGoCrazy());
    if(lovesmenotwonder==1)
    {
      StartCoroutine(FixLovesMeText());
    }
    else{StartCoroutine(ResetTextOnWin());}
  }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use `!{0} press` to press the button labeled 'PRESS ME'. Use `!{0} submit loves [me/me not] ` to submit your answer";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
     Command = Command.ToUpper();
    yield return null;

    switch (Command)
    {
        case "PRESS":
            Buttons[2].OnInteract();
            break;
         case "SUBMIT LOVES ME":
             Buttons[0].OnInteract();
             break;
         case "SUBMIT LOVES ME NOT":
             Buttons[1].OnInteract();
             break;
   }
 }

   IEnumerator TwitchHandleForcedSolve () {
     yield return new WaitForSeconds(0.5f);
     DisplayTexts[3].text = "";
     int twitchAnswer = buttonAnswer;
     if(twitchAnswer == 1){
       Buttons[1].OnInteract();
     }
     else
     {
       Buttons[0].OnInteract();
     }
      yield return null;
   }
}
