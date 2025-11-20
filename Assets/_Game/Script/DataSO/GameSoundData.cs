using UnityEngine;
using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;


[CreateAssetMenu(fileName = "GameSoundData", menuName = "_GAME/SFX/GameSoundData")]
public class GameSoundData : SingletonResourcesScriptable<GameSoundData>
{
    protected override void Initialize()
    {

    }
    [Header("Background music")]
    public AudioClip HomeBackground;
    public AudioClip DalganoBackground;
    public AudioClip GlassBrigdeBackground;
    public AudioClip LightOffBackground;
    public AudioClip LobbyBackground;
    public AudioClip PassOrDieBackground;
    public AudioClip SixLegBackground;
    public AudioClip SquidGameBackground;
    public AudioClip GreenLightBackground;
    public AudioClip RockPaperBackground;
    public AudioClip DontWakeUpBackground;
    [Space]
    [Header("SFX")]
    [Header("--------------------------")]
    [Header("General SFXs")]
    public AudioClip ButtonClick;
    [Space]
    [Header("Minigame SFXs")]
    public AudioClip HoldingPet;
    public AudioClip EarthImpact;
    public AudioClip FireballExplosion;
    public AudioClip GrenadeExplosion;
    public AudioClip Carousel;
    public AudioClip RedLight;
    public AudioClip GreenLight;
    public AudioClip Home;
    public AudioClip GunMisfire;
    public AudioClip PistolShot;
    public AudioClip PlayerDeath1;
    public AudioClip PlayerDeath2;
    public AudioClip PlayerDeath3;
    public AudioClip PlayerFall1;
    public AudioClip PlayerFall2;
    public AudioClip PlayerFall3;
    public AudioClip PlayerFall4;
    public AudioClip RevolverSpin;
    public AudioClip BallFalling;
    public AudioClip BladeBall_AttackBall;
    public AudioClip BladeBall_Slash;
    public AudioClip BladeBall_Solo;
    public AudioClip CandyCrack1;
    public AudioClip CandyCrack2;
    public AudioClip CandyCrack3;
    public AudioClip CharacterDie;
    public AudioClip CharacterFootsteps;
    public AudioClip CharacterJump;
    public AudioClip Checkpoint;
    public AudioClip CoinDrop;
    public AudioClip CountDown;
    public AudioClip DollRotation;
    public AudioClip GetPoint;
    public AudioClip LightOff;
    public AudioClip LoseMinigame;
    public AudioClip NextGate;
    public AudioClip PickWeapon;
    public AudioClip PigBreak;
    public AudioClip PlatformFade1;
    public AudioClip PlatformFade2;
    public AudioClip PlatformFade3;
    public AudioClip PlatformFade4;
    public AudioClip PlatformFade5;
    public AudioClip PlatformFade6;
    public AudioClip PlatformFade7;
    public AudioClip PlatformFade8;
    public AudioClip PlatformGlassBroken1;
    public AudioClip PlatformGlassBroken1_0;
    public AudioClip PlatformGlassBroken2;
    public AudioClip PlatformGlassBroken2_0;
    public AudioClip PlatformGlassBroken3;
    public AudioClip PlatformGlassBroken3_0;
    public AudioClip PlatformGlassBroken4;
    public AudioClip PlatformGlassBroken4_0;
    public AudioClip ReadLightGreenLightCountdown;
    public AudioClip Shoot1;
    public AudioClip Shoot1_0;
    public AudioClip Shoot2;
    public AudioClip Shoot2_0;
    public AudioClip Slash;
    public AudioClip Smash;
    public AudioClip SmashLight;
    public AudioClip Spin;
    public AudioClip Stab;
    public AudioClip ThrowBall;
    public AudioClip TugTap;
    public AudioClip Victory;
    public AudioClip TugVoice;
    public AudioClip WinMinigame;
    public AudioClip WinVoice;
    public AudioClip Rust;
    public AudioClip Throw;
    public AudioClip Spawn;
    public AudioClip Coin;
    public AudioClip Alert;
    public AudioClip PickupPet;
}

