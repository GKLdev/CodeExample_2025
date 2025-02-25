using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ReferenceDb_Public
{
    public interface IReferenceDb : IModuleInit
    {
        /// <summary>
        /// Retrieve entry from Db.
        /// </summary>
        /// <param name="_id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetEntry<T>(int _id) where T : DbEntryBase;

        public T GetEntry<T>(string _alias) where T : DbEntryBase;
    }
}
