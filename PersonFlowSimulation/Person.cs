namespace PersonFlowSimulation;

/// <summary>
/// VRフリーローム体験中にアリーナ内を移動する一人の体験者を表します。
/// Represents one participant moving in the VR freeroam arena.
/// </summary>
public class Person
{
    private const double DefaultTurnNoise = Math.PI / 4;

    public int Id { get; }

    /// <summary>体験者の年齢 (10〜60歳)。Age of the participant (10–60).</summary>
    public int Age { get; }

    /// <summary>年齢に基づく歩行速度 (m/s)。Walking speed based on age (m/s).</summary>
    public double WalkingSpeedMps { get; }

    /// <summary>シミュレーション開始からの累積移動距離 (m)。Total distance traveled (m).</summary>
    public double DistanceTraveled { get; private set; }

    public double X { get; private set; }
    public double Y { get; private set; }

    // ランダムウォーク用パラメータ
    private double _directionAngle;          // 現在の進行方向 (radians)
    private readonly double _stepSize;       // 1ステップあたりの移動距離 (m)
    private readonly double _turnNoise;      // 方向変化のランダム幅 (radians)

    private readonly Random _rng;

    /// <param name="stepSizeMeters">1ステップあたりの移動距離 = 歩行速度 × 時間刻み幅 (m)。
    /// Distance per simulation step = walking speed × seconds-per-step (m).</param>
    public Person(int id, int age, double startX, double startY, Random rng,
                  double stepSizeMeters, double turnNoise = DefaultTurnNoise)
    {
        Id = id;
        Age = age;
        WalkingSpeedMps = GetWalkingSpeedMps(age);
        X = startX;
        Y = startY;
        _rng = rng;
        _stepSize = stepSizeMeters;
        _turnNoise = turnNoise;
        _directionAngle = rng.NextDouble() * 2 * Math.PI;
        DistanceTraveled = 0;
    }

    /// <summary>
    /// 年齢から一般的な屋内歩行速度 (m/s) を返します。10歳〜60歳の計測値に基づきます。
    /// Returns typical indoor walking speed (m/s) for the given age, based on measured data for ages 10–60.
    /// </summary>
    public static double GetWalkingSpeedMps(int age)
    {
        // 子ども (10〜14歳): ~1.10 m/s
        if (age <= 14) return 1.10;
        // 青少年 (15〜19歳): 1.10 → 1.40 m/s へ線形増加
        if (age <= 19) return 1.10 + (age - 14) * 0.06;
        // 成人 (20〜40歳): ピーク ~1.40 m/s
        if (age <= 40) return 1.40;
        // 中高年 (41〜50歳): 1.40 → 1.35 m/s へ緩やかに低下
        if (age <= 50) return 1.40 - (age - 40) * 0.005;
        // 高齢 (51〜60歳): 1.35 → 1.15 m/s へ低下
        if (age <= 60) return 1.35 - (age - 50) * 0.02;
        return 1.15;
    }

    /// <summary>
    /// 一ステップ移動します。壁に当たると反射します。
    /// Moves one step. Reflects off walls.
    /// </summary>
    public void Move(double spaceWidth, double spaceHeight)
    {
        // 方向を少しランダムに変える (相関ランダムウォーク)
        _directionAngle += (_rng.NextDouble() * 2 - 1) * _turnNoise;

        double newX = X + Math.Cos(_directionAngle) * _stepSize;
        double newY = Y + Math.Sin(_directionAngle) * _stepSize;

        // X方向の壁反射
        if (newX < 0)
        {
            newX = -newX;
            _directionAngle = Math.PI - _directionAngle;
        }
        else if (newX > spaceWidth)
        {
            newX = 2 * spaceWidth - newX;
            _directionAngle = Math.PI - _directionAngle;
        }

        // Y方向の壁反射
        if (newY < 0)
        {
            newY = -newY;
            _directionAngle = -_directionAngle;
        }
        else if (newY > spaceHeight)
        {
            newY = 2 * spaceHeight - newY;
            _directionAngle = -_directionAngle;
        }

        // 境界内にクランプ（浮動小数点誤差対策）
        X = Math.Clamp(newX, 0, spaceWidth);
        Y = Math.Clamp(newY, 0, spaceHeight);

        DistanceTraveled += _stepSize;
    }
}
