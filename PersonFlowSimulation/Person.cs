namespace PersonFlowSimulation;

/// <summary>
/// スペース内を移動する一人の人を表します。
/// Represents one person moving in the space.
/// </summary>
public class Person
{
    private const double DefaultTurnNoise = Math.PI / 4;

    public int Id { get; }
    public double X { get; private set; }
    public double Y { get; private set; }

    // ランダムウォーク用パラメータ
    private double _directionAngle;          // 現在の進行方向 (radians)
    private readonly double _stepSize;       // 1ステップあたりの移動距離 (m)
    private readonly double _turnNoise;      // 方向変化のランダム幅 (radians)

    private readonly Random _rng;

    public Person(int id, double startX, double startY, Random rng,
                  double stepSize = 0.5, double turnNoise = DefaultTurnNoise)
    {
        Id = id;
        X = startX;
        Y = startY;
        _rng = rng;
        _stepSize = stepSize;
        _turnNoise = turnNoise;
        _directionAngle = rng.NextDouble() * 2 * Math.PI;
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
    }
}
