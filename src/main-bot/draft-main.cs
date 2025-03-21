using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;

public class TemplateBot : Bot
{   
    /* A bot that drives forward and backward, and fires a bullet */
    static void Main(string[] args)
    {
        new TemplateBot().Start();
    }

    TemplateBot() : base(BotInfo.FromFile("MyBot.json")) { }
    public bool setTarget;
    public int targetID;
    int turnDirection = 1; // clockwise (-1) or counterclockwise (1)


    public override void Run()
    {
        /* Customize bot colors, read the documentation for more information */
        InitializeRound();
        // AdjustGunForBodyTurn = true;

        while (IsRunning)
        {
            HandleMove();
            HandleGun();
        }
    }

    public void HandleMove(){
        Forward(5);
        if (setTarget){
            // TargetSpeed = MaxSpeed;
            Forward(100);
        }
    }

    public void HandleGun(){
        if (!setTarget){
            // GunTurnRate = MaxGunTurnRate;
            TurnLeft(360 * turnDirection);
            // SetTurnGunRight(360);
        } else {
            TurnRight(15 * turnDirection);
            // TurnGunRight(30);
            TurnLeft(35 * turnDirection);
        }
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        setTarget = true;
        targetID = e.ScannedBotId;  
        TurnToFaceTarget(e.X, e.Y);
        var distance = DistanceTo(e.X, e.Y);

        Rescan(); // Might want to move forward again!
        SetFire(1); //set based on distance
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

    public override void OnBotDeath(BotDeathEvent e){
        if (e.VictimId == targetID){
            setTarget = false;
        }
    }

    public override void OnHitBot(HitBotEvent e)
    {
        Console.WriteLine("Ouch! I hit a bot at " + e.X + ", " + e.Y);
    }

    public override void OnHitWall(HitWallEvent e)
    {
        Console.WriteLine("Ouch! I hit a wall, must turn back!");
    }

    /* Read the documentation for more events and methods */
    public void InitializeRound(){
        BodyColor = Color.Green;
        TurretColor = Color.Yellow;
        RadarColor = Color.Red;
        BulletColor = Color.Black;
        ScanColor = Color.Red;
        TracksColor = Color.Black;
        GunColor = Color.White;

        // SpecialRadarHandler = false;
        // SpecialMoveHandler = false;
        // FirstTurn = true;
        setTarget = false;
        targetID = -1;
    }
}
