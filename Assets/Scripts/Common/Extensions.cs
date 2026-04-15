using System.Collections.Generic;
using System.Linq;

public static class Extension
{
    public static bool ContainsSubSequence<T>(this IEnumerable<T> self, IEnumerable<T> other)
    {
        if (other.Count() == 0) return true;
        if (other.Count() > self.Count()) return false;

        for (int i = 0; i <= self.Count() - other.Count(); i++)
        {
            if (self.Skip(i).Take(other.Count()).SequenceEqual(other))
                return true;
        }
        return false;
    }
}