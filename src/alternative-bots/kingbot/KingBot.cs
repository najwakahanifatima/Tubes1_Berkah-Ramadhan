using System;
using System.Drawing;
using Robocode.TankRoyale.BotApi;
using Robocode.TankRoyale.BotApi.Events;
using System.IO;
using Microsoft.Extensions.Configuration;

// ------------------------------------------------------------------
// King Bot
// ------------------------------------------------------------------
// Optimalisasi Spin Bot.
// ------------------------------------------------------------------
public class KingBot : Bot
{
    int i;
    int div = 2;
    static void Main()
    {
        new KingBot().Start();
    }

    KingBot() : base(BotInfo.FromFile("KingBot.json")) { }

    public override void Run()
    {
        BodyColor = Color.Blue;
        TurretColor = Color.Yellow;
        RadarColor = Color.Yellow;
        BulletColor = Color.Blue;
        ScanColor = Color.Blue;

        TurnLeft(BearingTo(ArenaWidth/div, ArenaHeight/div));
        Forward(DistanceTo(ArenaWidth/div, ArenaHeight/div));

        i = 0;
        while (IsRunning)
        {
            
            if(i % 200 == 0){
                TurnLeft(BearingTo(ArenaWidth/div, ArenaHeight/div));
                Forward(DistanceTo(ArenaWidth/div, ArenaHeight/div));
            }else{
                SetTurnLeft(50_000);
                MaxSpeed = 5;
                Forward(50_000);
            }
            
            i+=1;
        }
    }

    public override void OnHitBot(HitBotEvent e)
    {
        Back(50);
        TurnLeft(BearingTo(ArenaWidth/div, ArenaHeight/div));
        Forward(90);
    }

    public override void OnHitByBullet(HitByBulletEvent e){
        Forward(50);
    }

    public override void OnScannedBot(ScannedBotEvent e)
    {
        Stop();
        Fire(3);
        Resume();
    }

    // private void SmartFire(double distance)
    // {
    //     if (distance > 200 || Energy < 15)
    //         Fire(1);
    //     else if (distance > 50)
    //         Fire(2);
    //     else
    //         Fire(3);
    // }
}