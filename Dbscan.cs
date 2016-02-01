using System;
using System.Collections.Generic;

/// <summary>
/// Contains an implementation of the DBSCAN algorithm. This class cannot be inherited.
/// </summary>
public class DbscanAlgorithm
{

    #region constructors

    private DbscanAlgorithm()
    {
    }

    #endregion

    #region methods

    /// <summary>
    /// Clusters the specified points using the specified value and minimum points to form a cluster.
    /// </summary>
    /// <param name="points">The points to cluster.</param>
    /// <param name="eps">The value to use to find neighoring points.</param>
    /// <param name="minimumClusterCount">The minimum number of points to form a cluster.</param>
    /// <returns>The number of clusters created from the collection.</returns>
    public static int Dbscan<XType, YType>(IDbscanPoint<XType, YType>[] points, double eps, int minimumClusterCount)
    {
        int cid = 0;

        foreach (IDbscanPoint<XType, YType> p in points)
        {
            if (!p.IsVisited)
            {
                p.IsVisited = true;

                IDbscanPoint<XType, YType>[] neighbors = DbscanAlgorithm.GetNeighors(points, p, eps);

                if (neighbors.Length < minimumClusterCount)
                    p.IsNoise = true;
                else
                {
                    cid++;
                    DbscanAlgorithm.ExpandCluster(points, p, neighbors, cid, eps, minimumClusterCount);
                }
            }
        }

        return cid;
    }

    private static void ExpandCluster<XType, YType>(IDbscanPoint<XType, YType>[] points, IDbscanPoint<XType, YType> p, IDbscanPoint<XType, YType>[] neighbors, int cid, double eps, int minimumClusterCount)
    {
        p.ClusterId = cid;

        Queue<IDbscanPoint<XType, YType>> q = new Queue<IDbscanPoint<XType, YType>>(neighbors);

        while (q.Count > 0)
        {
            IDbscanPoint<XType, YType> n = q.Dequeue();
            if (!n.IsVisited)
            {
                n.IsVisited = true;

                IDbscanPoint<XType, YType>[] ns = DbscanAlgorithm.GetNeighors(points, n, eps);
                if (ns.Length >= minimumClusterCount)
                    foreach (IDbscanPoint<XType, YType> item in ns)
                        q.Enqueue(item);
            }
            else if (!n.ClusterId.HasValue)
                n.ClusterId = cid;
        }
    }

    private static IDbscanPoint<XType, YType>[] GetNeighors<XType, YType>(IDbscanPoint<XType, YType>[] points, IDbscanPoint<XType, YType> point, double eps)
    {
        List<IDbscanPoint<XType, YType>> neighbors = new List<IDbscanPoint<XType, YType>>();
        neighbors.Add(point);

        foreach (IDbscanPoint<XType, YType> p in points)
            if (point.IsNeighbor(p, eps))
                neighbors.Add(p);

        return neighbors.ToArray();
    }

    #endregion

}

/// <summary>
/// Contains an implementation of the <see cref="IDbscanPoint"/> interface 
/// using <see cref="double"/> types for the X and Y components.
/// </summary>
[System.Diagnostics.DebuggerDisplay(@"\{X={X}, Y={Y}\}")]
public class DbscanPoint : IDbscanPoint<double, double>
{

    #region properties

    /// <summary>
    /// Gets or sets the X component of the point.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the Y component of the point.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the point is noise.
    /// </summary>
    public bool IsNoise { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the point was visited.
    /// </summary>
    public bool IsVisited { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the cluster id.
    /// </summary>
    public int? ClusterId { get; set; }

    #endregion

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DbscanPoint"/> class.
    /// </summary>
    public DbscanPoint()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DbscanPoint"/> class with the specified values.
    /// </summary>
    /// <param name="x">The X component of the point.</param>
    /// <param name="y">The Y component of the point.</param>
    public DbscanPoint(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }

    #endregion

    #region methods

    /// <summary>
    /// Determines whether the specified point neighbors the current instance using the specified value.
    /// </summary>
    /// <param name="point">The point to test as a neighor.</param>
    /// <param name="eps">The value to use to find neighoring points.</param>
    /// <returns>True if the point is a neighbor; otherwise, false.</returns>
    public bool IsNeighbor(IDbscanPoint<double, double> point, double eps)
    {
        return (Math.Pow(point.X - this.X, 2) + Math.Pow(point.Y - this.Y, 2) < Math.Pow(eps, 2));
    }

    #endregion

}

/// <summary>
/// Contains the functionality an object requires to perform a DBSCAN.
/// </summary>
public interface IDbscanPoint<XType, YType>
{

    #region properties

    /// <summary>
    /// Gets or sets the X component of the point.
    /// </summary>
    XType X { get; set; }

    /// <summary>
    /// Gets or sets the Y component of the point.
    /// </summary>
    YType Y { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the point is noise.
    /// </summary>
    bool IsNoise { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the point was visited.
    /// </summary>
    bool IsVisited { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the cluster id.
    /// </summary>
    int? ClusterId { get; set; }

    #endregion

    #region methods

    /// <summary>
    /// Determines whether the specified point neighbors the current instance using the specified value.
    /// </summary>
    /// <param name="point">The point to test as a neighor.</param>
    /// <param name="eps">The value to use to find neighoring points.</param>
    /// <returns>True if the point is a neighbor; otherwise, false.</returns>
    bool IsNeighbor(IDbscanPoint<XType, YType> point, double eps);

    #endregion

}