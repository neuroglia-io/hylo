namespace Hylo;

/// <summary>
/// Represents an <see cref="Exception"/> thrown by an Hylo component
/// </summary>
public class HyloException
    : Exception
{

    /// <summary>
    /// Initializes a new <see cref="HyloException"/>
    /// </summary>
    /// <param name="problem">An object that describes the problem that has occured</param>
    public HyloException(ProblemDetails problem)
        : base($"[{problem.Status} - {problem.Title}] {problem.Detail}{(problem.Errors?.Any() == true ? Environment.NewLine + string.Join(Environment.NewLine, problem.Errors.Select(e => $"{e.Key}: {string.Join(", ", e.Value)}")) : "")}")
    {
        this.Problem = problem;
    }

    /// <summary>
    /// An object that describes the problem that has occured
    /// </summary>
    public ProblemDetails Problem { get; }

}
