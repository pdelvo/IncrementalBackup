using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IncrementalBackup.Tests
{
    public static class TestExtensions
    {
        public static void Throws<T>(Action action) where T: Exception
        {
            try
            {
                action();
                throw new AssertFailedException(string.Format("Expression does not throw a exception of type '{0}'",
                                                              typeof(T)));
            }
            catch (T)
            {

            }
            catch (Exception)
            {
                throw new AssertFailedException(string.Format("Expression does not throw a exception of type '{0}'",
                                                              typeof (T)));
            }
        }
    }
}
