using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

public class GritBot : Bot
{   
    static void Main(string[] args)
    {
        new GritBot().Start();
    }

    GritBot() : base(BotInfo.FromFile("GritBot.json")) { }
    public bool setTarget;
    public int targetID;
    int turnDirection = 1;
    public List<Enemy> enemies = new List<Enemy>();


    public override void Run()
    {
        InitializeRound();
        while (IsRunning)
        {
            HandleMove();
            HandleGun();
        }
    }

    public void HandleMove(){
        Forward(5);
        if (setTarget){
            Forward(100);
        }
    }

    public void HandleGun(){
        if (!setTarget){
            TurnGunLeft(360 * turnDirection);
        } else {
            TurnGunRight(20 * turnDirection);
            TurnGunLeft(45 * turnDirection);
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        setTarget = true;
        Console.WriteLine("A Bot is scanned");

        Enemy scannedEnemy = enemies.FirstOrDefault(en => en.id == e.ScannedBotId);
        var enemyDistance = DistanceTo(e.X, e.Y);
        var enemyDirection = DirectionTo(e.X, e.Y);
        if (scannedEnemy == null) {
            enemies.Add(new Enemy(e.ScannedBotId, e.Energy, e.X, e.Y));
            Console.WriteLine("New enemy scanned!");
        } else {
            Console.WriteLine("Scanned enemy is " + e.ScannedBotId);
            if (scannedEnemy.IsEnergyEnemyDec(e.Energy)){
                Console.WriteLine("Enemy's energy is decreasing!");
                DodgeMovement();
            }
        }

        FireTarget(enemyDistance, enemyDirection);
    }

        public void FireTarget(double distance, double enemyDirection){
        var gunBearing = NormalizeRelativeAngle(enemyDirection - GunDirection);
        SetTurnGunLeft(gunBearing);
        if (distance > 200 || Energy < 20){
            for (int i = 0; i < 7; i++){
                Fire(1);
            }
        } else if (distance > 100){
            Fire(2);
            Fire(2);
        } else {
            Fire(3);
        }
    }

    private void TurnToFaceTarget(double x, double y)
    {
        var bearing = BearingTo(x, y);
        if (bearing >= 0)
            turnDirection = 1;
        else
            turnDirection = -1;

        TurnLeft(bearing);
    }

    public void DodgeMovement(){
        SetTurnRight(45);
        SetForward(50);
    }

    public override void OnBotDeath(BotDeathEvent e){
        if (e.VictimId == targetID){
            setTarget = false;
        }
    }

    public override void OnHitBot(HitBotEvent e)
    {
        TurnRight(90);
        Forward(30);
        Console.WriteLine("Ouch! I hit a bot at " + e.X + ", " + e.Y);
    }

    public override void OnHitWall(HitWallEvent e)
    {
        TurnRight(90);
        Back(10);
        Console.WriteLine("Ouch! I hit a wall, must turn back!");
    }

    // Inititalize Color and Variables
    public void InitializeRound(){
        BodyColor = Color.Blue;
        TurretColor = Color.Yellow;
        RadarColor = Color.Red;
        BulletColor = Color.Black;
        ScanColor = Color.Pink;
        TracksColor = Color.Black;
        GunColor = Color.White;

        setTarget = false;
        targetID = -1;
    }
}

public class Enemy{
    public int id {get; set;}
    private double energy {get; set;}
    private double x {get; set;}
    private double y {get; set;}

    public Enemy(int id, double energy, double x, double y){
        this.id = id;
        this.energy = energy;
        this.x = x;
        this.y = y;
    }

    public void Update(double energy, double x, double y){
        this.energy = energy;
        this.x = x;
        this.y = y;
    }

    public bool IsEnergyEnemyDec(double curEnergy){
        return curEnergy < this.energy;
    }
}
