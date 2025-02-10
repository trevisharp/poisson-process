using System;
using System.Linq;

var X = monteCarlo(.5f, 10_000);
var Y = poisson(5f);

var error = X.Take(15).Zip(Y.Take(15))
    .Select(t => t.First - t.Second)
    .Select(MathF.Abs)
    .Average();

Console.WriteLine(100 * error);

float[] poisson(float lambda)
{
    const float time = 10f;
    int maxExpected = (int)(5 * time * lambda);
    var dist = new float[maxExpected];

    var explambda = MathF.Exp(-lambda);
    for (int k = 0; k < dist.Length; k++)
        dist[k] = MathF.Pow(lambda, k) * explambda / factorial(k);
    
    return dist;
}

int factorial(int k)
{
    int result = 1;
    while (k > 1)
    {
        result *= k;
        k--;
    }
    return result;
}

float[] monteCarlo(float lambda, int k)
{
    const float time = 10f;
    int maxExpected = (int)(5 * time * lambda);
    var dist = new float[maxExpected];
    var process = new PoissonProcess(lambda);

    for (int i = 0; i < k; i++)
    {
        int matches = process.Next(time);
        if (matches >= maxExpected)
            continue;
        
        dist[matches]++;
    }

    for (int i = 0; i < dist.Length; i++)
        dist[i] /= k;

    return dist;
}

public class PoissonProcess(float lambda)
{
    const float dt = 1E-3f;
    public int Next()
    {
        int matches = 0;
        float expected = dt * lambda;
        
        while (Random.Shared.NextSingle() < expected)
            matches++;
        
        return matches;
    }

    public int Next(float time)
    {
        int count = 0;
        for (float t = 0; t < time; t += dt)
            count += Next();
        return count;
    }
}