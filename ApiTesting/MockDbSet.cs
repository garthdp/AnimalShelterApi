using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTesting
{
    public class MockDbSet<T> : Mock<DbSet<T>> where T : class
    {
        /*
        Code Attribution
        Title: NSubstitute DbSet / IQueryable<T>
        Author: s.meijer
        Link: https://stackoverflow.com/questions/21069986/nsubstitute-dbset-iqueryablet
        Usage: Used to understand make a general purpose mock dbset
        */
        public MockDbSet(IQueryable<T> data)
        {
            As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }
    }

}
