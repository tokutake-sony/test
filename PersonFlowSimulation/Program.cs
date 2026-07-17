using PersonFlowSimulation;

// ============================================================
// VRフリーローム人流シミュレーション
// VR Freeroam Person Flow Simulation
//
// コンテンツ時間: 60分
// 体験者: 100人（10〜60歳・均等分布）
// 歩行速度: 年齢ごとの一般的な屋内歩行速度を使用
// Content duration : 60 minutes
// Participants      : 100 people (ages 10–60, uniform distribution)
// Walking speed     : Typical indoor walking speed per age group
// ============================================================

const int ContentMinutes = 60;        // コンテンツの長さ (分)
const int TotalSteps     = ContentMinutes; // 1ステップ = 1分
const int PrintInterval  = 10;        // ヒートマップ表示間隔 (分)

var simulation = new Simulation(
    spaceWidth:  31.62,               // √1000 ≈ 31.62 m
    spaceHeight: 31.62,
    personCount: 100,
    seed: 42
);

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=================================================================");
Console.WriteLine("  VRフリーローム 人流シミュレーション");
Console.WriteLine("  VR Freeroam Person Flow Simulation");
Console.WriteLine($"  スペース: 1000㎡  体験者: 100人  コンテンツ長: {ContentMinutes}分");
Console.WriteLine($"  1ステップ = {Simulation.SecondsPerStep}秒 (1分)  総ステップ数: {TotalSteps}");
Console.WriteLine("=================================================================");
Console.WriteLine();

Console.WriteLine("【参加者の年齢層と歩行速度】 Age Distribution & Walking Speed");
simulation.PrintAgeDistribution();
Console.WriteLine();

for (int step = 0; step <= TotalSteps; step++)
{
    if (step % PrintInterval == 0)
    {
        Console.WriteLine($"--- {step,3}分経過 / {ContentMinutes}分 ---");
        simulation.PrintHeatmap(gridCols: 32, gridRows: 16);
        simulation.PrintStats();
        Console.WriteLine();
    }

    if (step < TotalSteps)
        simulation.Step();
}

Console.WriteLine("=================================================================");
Console.WriteLine("  累積人流ヒートマップ (60分間の総移動軌跡)");
Console.WriteLine("  Cumulative Flow Heatmap – 60-minute total trajectories");
Console.WriteLine("=================================================================");
simulation.PrintCumulativeHeatmap(gridCols: 64, gridRows: 32);
Console.WriteLine();
Console.WriteLine("シミュレーション完了。 / Simulation complete.");

