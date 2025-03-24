using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
public class Syzygy : Bot
{
    static void Main(string[] args)
    {
        new Syzygy().Start();
    }

    Syzygy() : base(BotInfo.FromFile("Syzygy.json")) { }

    // Create enemy list
    public List<Enemy> enemies = new List<Enemy>();
    
    public override void Run()
    {
        InitializeRound();
    
        while (IsRunning)
        {
            GunTurnRate = MaxGunTurnRate;
            HandleRadar();
            HandleMove();
        }
    }

    public void HandleRadar(){
        SetTurnGunRight(360); 
    }

    public void HandleMove(){
        int randomForwBack = Random.Shared.Next(-75, 201);
        int randomTurn = Random.Shared.Next(-90,91);
        Forward(randomForwBack);
        SetTurnRight(randomTurn); 
    }

    public override void OnScannedBot(ScannedBotEvent e){
        Enemy scannedEnemy = enemies.FirstOrDefault(en => en.id == e.ScannedBotId);
        var enemyDistance = DistanceTo(e.X, e.Y);
        var enemyGunBearing = GunBearingTo(e.X, e.Y);
        if (scannedEnemy == null) {
            enemies.Add(new Enemy(e.ScannedBotId, e.Energy, e.X, e.Y));
        } else {
            if (scannedEnemy.IsEnergyEnemyDec(e.Energy)){
                DodgeMovement();
            }
        }
        FireTarget(enemyDistance, enemyGunBearing);
    }

    public void DodgeMovement(){
        SetTurnRight(45);
        SetForward(50);
    }

    public override void OnHitByBullet(HitByBulletEvent e){
        SetTurnRight(45);
        SetForward(50);
    }   

    public override void OnHitWall(HitWallEvent e){
        SetBack(20);
        SetTurnRight(90);
    }

    public override void OnHitBot(HitBotEvent e){
        SetBack(10);
        SetTurnLeft(90);
    }

    public void FireTarget(double distance, double gunBearing){
        if (distance > 200 || Energy < 20){
            Fire(1);
        } else if (distance > 100){
            Fire(2);
        } else {
            Fire(3);
        }
    }

    public void InitializeRound(){
        BodyColor = Color.Green;
        TurretColor = Color.Yellow;
        RadarColor = Color.Red;
        BulletColor = Color.Black;
        ScanColor = Color.Red;
        TracksColor = Color.Black;
        GunColor = Color.White;

        enemies.Clear();
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