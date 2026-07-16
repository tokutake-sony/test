namespace PersonFlowSimulation;

/// <summary>
/// シミュレーション全体を管理します。
/// Manages the entire simulation.
/// </summary>
public class Simulation
{
    public double SpaceWidth { get; }
    public double SpaceHeight { get; }
    public int PersonCount { get; }
    public int CurrentStep { get; private set; }

    private readonly List<Person> _people;
    private readonly Random _rng;

    // 累積ヒートマップ（各グリッドセルへの訪問回数）
    private const int HeatmapGridSize = 100;
    private readonly int[,] _cumulativeHeat = new int[HeatmapGridSize, HeatmapGridSize];

    public Simulation(double spaceWidth, double spaceHeight, int personCount, int seed = 0)
    {
        SpaceWidth = spaceWidth;
        SpaceHeight = spaceHeight;
        PersonCount = personCount;
        CurrentStep = 0;

        _rng = new Random(seed);
        _people = new List<Person>(personCount);

        // 100人をランダムな初期位置に配置
        for (int i = 0; i < personCount; i++)
        {
            double x = _rng.NextDouble() * spaceWidth;
            double y = _rng.NextDouble() * spaceHeight;
            _people.Add(new Person(i, x, y, new Random(_rng.Next())));
        }

        // 初期位置をヒートマップに記録
        RecordPositions();
    }

    /// <summary>1ステップ進めます。</summary>
    public void Step()
    {
        foreach (var person in _people)
            person.Move(SpaceWidth, SpaceHeight);

        RecordPositions();
        CurrentStep++;
    }

    private void RecordPositions()
    {
        foreach (var person in _people)
        {
            int col = (int)(person.X / SpaceWidth * HeatmapGridSize);
            int row = (int)(person.Y / SpaceHeight * HeatmapGridSize);
            col = Math.Clamp(col, 0, HeatmapGridSize - 1);
            row = Math.Clamp(row, 0, HeatmapGridSize - 1);
            _cumulativeHeat[row, col]++;
        }
    }

    /// <summary>
    /// 現在位置のヒートマップをコンソールに表示します。
    /// Prints a heatmap of current positions to the console.
    /// </summary>
    public void PrintHeatmap(int gridCols = 32, int gridRows = 16)
    {
        // 現在位置のカウント
        var grid = new int[gridRows, gridCols];
        foreach (var person in _people)
        {
            int col = (int)(person.X / SpaceWidth * gridCols);
            int row = (int)(person.Y / SpaceHeight * gridRows);
            col = Math.Clamp(col, 0, gridCols - 1);
            row = Math.Clamp(row, 0, gridRows - 1);
            grid[row, col]++;
        }

        int maxVal = 1;
        for (int r = 0; r < gridRows; r++)
            for (int c = 0; c < gridCols; c++)
                maxVal = Math.Max(maxVal, grid[r, c]);

        // ボーダー上
        Console.Write("┌");
        Console.Write(new string('─', gridCols));
        Console.WriteLine("┐");

        for (int r = 0; r < gridRows; r++)
        {
            Console.Write("│");
            for (int c = 0; c < gridCols; c++)
            {
                int count = grid[r, c];
                Console.Write(GetDensityChar(count, maxVal));
            }
            Console.WriteLine("│");
        }

        // ボーダー下
        Console.Write("└");
        Console.Write(new string('─', gridCols));
        Console.WriteLine("┘");

        PrintLegend();
    }

    /// <summary>
    /// 累積人流ヒートマップをコンソールに表示します。
    /// Prints the cumulative flow heatmap to the console.
    /// </summary>
    public void PrintCumulativeHeatmap(int gridCols = 64, int gridRows = 32)
    {
        // 内部グリッドから指定サイズに集計
        var grid = new int[gridRows, gridCols];
        for (int r = 0; r < HeatmapGridSize; r++)
        {
            for (int c = 0; c < HeatmapGridSize; c++)
            {
                int targetRow = r * gridRows / HeatmapGridSize;
                int targetCol = c * gridCols / HeatmapGridSize;
                grid[targetRow, targetCol] += _cumulativeHeat[r, c];
            }
        }

        int maxVal = 1;
        for (int r = 0; r < gridRows; r++)
            for (int c = 0; c < gridCols; c++)
                maxVal = Math.Max(maxVal, grid[r, c]);

        Console.Write("┌");
        Console.Write(new string('─', gridCols));
        Console.WriteLine("┐");

        for (int r = 0; r < gridRows; r++)
        {
            Console.Write("│");
            for (int c = 0; c < gridCols; c++)
            {
                int val = grid[r, c];
                Console.Write(GetDensityChar(val, maxVal));
            }
            Console.WriteLine("│");
        }

        Console.Write("└");
        Console.Write(new string('─', gridCols));
        Console.WriteLine("┘");

        Console.WriteLine($"  (最大密度: {maxVal} 通過, スペース: {SpaceWidth:F1}m × {SpaceHeight:F1}m = {SpaceWidth * SpaceHeight:F0}㎡)");
        PrintLegend();
    }

    /// <summary>現在のステップの統計情報を表示します。</summary>
    public void PrintStats()
    {
        double avgX = _people.Average(p => p.X);
        double avgY = _people.Average(p => p.Y);
        double stdX = Math.Sqrt(_people.Average(p => (p.X - avgX) * (p.X - avgX)));
        double stdY = Math.Sqrt(_people.Average(p => (p.Y - avgY) * (p.Y - avgY)));

        Console.WriteLine($"  人数: {PersonCount}人  重心: ({avgX:F2}, {avgY:F2})  " +
                          $"分散[σ]: X={stdX:F2}m, Y={stdY:F2}m");
    }

    // 密度に応じた文字を返します
    private static char GetDensityChar(int count, int maxVal)
    {
        if (count == 0) return ' ';
        double ratio = (double)count / maxVal;
        return ratio switch
        {
            < 0.1 => '·',
            < 0.2 => '░',
            < 0.4 => '▒',
            < 0.6 => '▓',
            < 0.8 => '█',
            _ => '■'
        };
    }

    private static void PrintLegend()
    {
        Console.WriteLine("  密度凡例: [ ] 0人  [·] 低  [░] やや低  [▒] 中  [▓] やや高  [█] 高  [■] 最高");
    }
}
