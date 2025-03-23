using System;
using System.Collections.Generic;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class Dreadfang : Bot
{
    private int turnDirection = 1;
    private Dictionary<int, double> enemies = new Dictionary<int, double>();
    private int targetId = -1; 
    private const double safeDistance = 25; 

    static void Main(string[] args)
    {
        new Dreadfang().Start();
    }

    Dreadfang() : base(BotInfo.FromFile("Dreadfang.json")) {}

    public override void OnGameStarted(GameStartedEvent e)
    {
        base.OnGameStarted(e);
        AdjustRadarForBodyTurn = true;
        //AdjustGunForBodyTurn = true;
    }

public override void Run()
{
    BodyColor = Color.FromArgb(0xFF, 0x14, 0x93);
    TurretColor = Color.Black;
    RadarColor = Color.Black;
    BulletColor = Color.Purple;
    ScanColor = Color.Black;

    while (IsRunning)
    {
        double turnAngle;
        if (enemies.Count > 10) turnAngle = IsNearWall() ? 20 : 40;
        else if(enemies.Count > 5) turnAngle = IsNearWall() ? 40 : 80;
        else turnAngle = IsNearWall() ? 60 : 120;

        TurnLeft(turnAngle * turnDirection);
        Forward(100);
        TargetSpeed = 5;
        RadarTurnRate = MaxRadarTurnRate;
        //GunTurnRate = MaxGunTurnRate;
    }
}
    public override void OnScannedBot(ScannedBotEvent e)
    {
        RadarTurnRate = MaxRadarTurnRate;
        //GunTurnRate = MaxGunTurnRate;
        enemies[e.ScannedBotId] = e.Energy;

        // Cari musuh dengan energi paling rendah
        if(targetId == -1 || !enemies.ContainsKey(targetId) || e.Energy < enemies[targetId]){
            targetId = e.ScannedBotId;
        }

        // Kalau ada target valid, fokuskan turret dan serang
        if(targetId != -1 && enemies.ContainsKey(targetId) && enemies.Count <= 1){
            TargetSpeed = 2;
            var target = enemies[targetId];
            TurnToFaceTarget(e.X, e.Y);
            if (DistanceTo(e.X, e.Y) > 300) Fire(0.2);
            else if (target > 200) Fire(0.5);
            else if (target > 150) Fire(1);
            else if (target > 80) Fire(2);
            else Fire(3);
        }
    }

    public override void OnHitByBullet(HitByBulletEvent e)
    {
        TargetSpeed = 8;
        SetTurnLeft(90*turnDirection);
        Forward(50);
    }

    public override void OnHitWall(HitWallEvent e)
    {
        turnDirection *= -1; // Ubah arah belokan saat menabrak tembok
        TargetSpeed = -TargetSpeed;
        TurnLeft(90);
        Forward(50);
    }

    public override void OnHitBot(HitBotEvent e)
    {
        TurnToFaceTarget(e.X, e.Y);
        if (e.Energy > 16) Fire(3);
        else if (e.Energy > 10) Fire(2);
        else if (e.Energy > 4) Fire(1);
        else if (e.Energy > 2) Fire(0.5);
        else if (e.Energy > 0.4) Fire(0.1);
        Forward(40);
    }

    private void TurnToFaceTarget(double x, double y)
    {
        var bearing = BearingTo(x, y);
        turnDirection = bearing >= 0 ? 1 : -1;
        TurnLeft(bearing);
    }

    private bool IsNearWall()
    {
        return (X < safeDistance || X > ArenaWidth-safeDistance ||
                Y < safeDistance || Y > ArenaHeight-safeDistance);
    }
}
