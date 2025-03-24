// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Drawing;
// using Robocode.TankRoyale.BotApi;
// using Robocode.TankRoyale.BotApi.Events;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Configuration.Json;

// public class TemplateBot : Bot
// {   
//     static void Main(string[] args)
//     {
//         new TemplateBot().Start();
//     }

//     TemplateBot() : base(BotInfo.FromFile("MyBot.json")) { }
//     public bool setTarget;
//     public int targetID;
//     int turnDirection = 1; // clockwise (-1) or counterclockwise (1)
//     int safeDistance = 15;

//     public List<Enemy> enemies = new List<Enemy>();

//     public override void Run()
//     {
//         InitializeRound();
//         RadarTurnRate = MaxRadarTurnRate;
//         GunTurnRate = MaxGunTurnRate;
//         while (IsRunning)
//         {
//             HandleRadar();
//             HandleMove();
//         }
//     }

//     public void HandleMove(){
//         if (IsNearWall()){
//             Forward(10);
//             TurnRight(90);
//         }
//         if (setTarget){
//             Console.WriteLine("1");
//             Rescan();
//         } else {
//             Forward(3);
//         }
//     }

//     public void HandleRadar(){
//         Console.WriteLine("Radar is being handled with setTarget is " + setTarget);
//         SetTurnRadarLeft(360 * turnDirection);
//         // if (!setTarget){
//         //     Console.WriteLine("here no set target");
//         // } else {
//         //     Console.WriteLine("here yes set target");
//         //     SetTurnRadarRight(10 * turnDirection);
//         //     SetTurnRadarLeft(25 * turnDirection);
//         // }
//     }

//     public override void OnScannedBot(ScannedBotEvent e)
//     {
//         setTarget = true;
//         Console.WriteLine("A Bot is scanned");
//         // TurnBodyToFaceTarget(e.X, e.Y);

//         Enemy scannedEnemy = enemies.FirstOrDefault(en => en.id == e.ScannedBotId);
//         var enemyDistance = DistanceTo(e.X, e.Y);
//         var enemyDirection = DirectionTo(e.X, e.Y);
//         if (scannedEnemy == null) {
//             enemies.Add(new Enemy(e.ScannedBotId, e.Energy, e.X, e.Y));
//             Console.WriteLine("New enemy scanned!");
//         } else {
//             Console.WriteLine("Scanned enemy is " + e.ScannedBotId);
//             if (scannedEnemy.IsEnergyEnemyDec(e.Energy)){
//                 Console.WriteLine("Enemy's energy is decreasing!");
//                 DodgeMovement();
//             }
//         }

//         FireTarget(enemyDistance, enemyDirection);
//         Rescan();
//     }

//     public void FireTarget(double distance, double enemyDirection){
//         Console.WriteLine("2");
//         var gunBearing = NormalizeRelativeAngle(enemyDirection - GunDirection);
//         SetTurnGunLeft(gunBearing);
//         if (distance > 200 || Energy < 20){
//             for (int i = 0; i < 5; i++){
//                 Fire(1);
//             }
//         } else if (distance > 100){
//             Fire(3);
//             Fire(1);
//         } else {
//             Fire(7);
//             Fire(3);
//         }
//     }

//     private void TurnBodyToFaceTarget(double x, double y)
//     {
//         var bearing = BearingTo(x, y);
//         if (bearing >= 0)
//             turnDirection = 1;
//         else
//             turnDirection = -1;

//         SetTurnLeft(bearing);
//         Console.WriteLine("3");
//     }

//     private bool IsNearWall()
//     {
//         return (X < safeDistance || X > ArenaWidth-safeDistance ||
//                 Y < safeDistance || Y > ArenaHeight-safeDistance);
//     }

//     public void DodgeMovement(){
//         Resume();
//         Console.WriteLine("4");
//         if (IsNearWall()){
//             Forward(10);
//             SetTurnRight(90);
//         } else {
//             SetTurnRight(45);
//             SetForward(50);
//         }
//     }

//     public override void OnBotDeath(BotDeathEvent e){
//         if (e.VictimId == targetID){
//             setTarget = false;
//         }
//     }

//     public override void OnHitBot(HitBotEvent e)
//     {
//         SetTurnRight(90);
//         Forward(30);
//         Console.WriteLine("Ouch! I hit a bot at " + e.X + ", " + e.Y);
//     }

//     public override void OnHitWall(HitWallEvent e)
//     {
//         Back(10);
//         TurnRight(90);
//         Console.WriteLine("Ouch! I hit a wall, must turn back!");
//     }

//     public void InitializeRound(){
//         BodyColor = Color.Green;
//         TurretColor = Color.Yellow;
//         RadarColor = Color.Red;
//         BulletColor = Color.Black;
//         ScanColor = Color.Red;
//         TracksColor = Color.Black;
//         GunColor = Color.White;

//         AdjustGunForBodyTurn = true;
//         AdjustRadarForGunTurn = true;
//         AdjustRadarForBodyTurn = true;

//         setTarget = false;
//         targetID = -1;
//     }
// }

// public class Enemy{
//     public int id {get; set;}
//     private double energy {get; set;}
//     private double x {get; set;}
//     private double y {get; set;}

//     public Enemy(int id, double energy, double x, double y){
//         this.id = id;
//         this.energy = energy;
//         this.x = x;
//         this.y = y;
//     }

//     public void Update(double energy, double x, double y){
//         this.energy = energy;
//         this.x = x;
//         this.y = y;
//     }

//     public bool IsEnergyEnemyDec(double curEnergy){
//         return curEnergy < this.energy;
//     }
// }
