using Godot;
using System;

public partial class Bomb : CharacterBody2D
{

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private CollisionShape2D Collider;
    private AnimatedSprite2D AnimatedSprite;

    private Area2D ExplosionArea;
    private CollisionShape2D ExplosionAreaCollider;
    private TextureProgressBar FuseProgressBar;
    private Timer FuseTimer;

    public bool IsFusing = false;
    public bool IsExploding = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        FuseProgressBar = GetNode<TextureProgressBar>("FuseProgressBar");
        ExplosionArea = GetNode<Area2D>("ExplosionArea");
        ExplosionAreaCollider = ExplosionArea.GetNode<CollisionShape2D>("ExplosionCollider");
        Collider = GetNode<CollisionShape2D>("Collider");
        FuseTimer = GetNode<Timer>("FuseTimer");
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        FuseTimer.Timeout += OnTimerTimeout;

        AnimatedSprite.AnimationFinished += OnAnimationFinished;

        ExplosionArea.Connect("body_entered", new Callable(this, "OnExplosionAreaEntered"));
        SignalBus.Instance.Connect("PlayerPickedUpBomb", new Callable(this, "BombPickedUp"));
    }

    public void BombPickedUp(Node2D bomb)
    {
        if (bomb == this)
        {
            QueueFree();
        }
    }

    private void OnTimerTimeout()
    {
        IsExploding = true;

    }

    private void OnAnimationFinished()
    {
        if (AnimatedSprite.Animation == "Explode")
        {
            QueueFree();
        }
    }

    private void OnExplosionAreaEntered(Node2D body)
    {
        if (IsExploding && body is Player)
        {
            SignalBus.Instance.EmitSignal("PlayerHitByExplosion");
        }
    }

    public override void _ExitTree()
    {
        FuseTimer.Timeout -= OnTimerTimeout;
        AnimatedSprite.AnimationFinished -= OnAnimationFinished;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y += gravity * (float)delta;
        }

        if (IsOnFloor() && !IsFusing)
        {
            FuseTimer.Start();
            IsFusing = true;
        }

        if (IsOnFloor() && IsFusing)
        {
            if (FuseProgressBar.Visible == false)
            {
                FuseProgressBar.Visible = true;
            }
            FuseProgressBar.Value = FuseTimer.TimeLeft / FuseTimer.WaitTime * 100;
        }

        if (IsExploding)
        {
            FuseProgressBar.Visible = false;
        }

        if (IsExploding && ExplosionAreaCollider.Disabled == true)
        {
            ExplosionAreaCollider.Disabled = false;
        }

        Velocity = velocity;

        MoveAndSlide();
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        if (IsExploding)
        {
            AnimatedSprite.Play("Explode");
        }
        else if (IsFusing)
        {
            AnimatedSprite.Play("Fuse");
        }
        else if (!IsOnFloor() && !IsFusing)
        {
            AnimatedSprite.Play("Idle");
        }
    }

}
