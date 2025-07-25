// AbstractFileSynchronizer.cs
// Contains the AbstractFileSynchronizer class

namespace FileUtils {
    public abstract class AbstractFileSynchronizer
    {
        public abstract void listSrcDirs();
        public abstract void listSrcFiles();
        public abstract void listDestDirs();
        public abstract void listDestFiles();
        public abstract void synchronize();
        public abstract void copy();
    }
}
