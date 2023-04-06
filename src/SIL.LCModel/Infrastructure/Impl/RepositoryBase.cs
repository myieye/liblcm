// Copyright (c) 2010-2017 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;

namespace SIL.LCModel.Infrastructure.Impl
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Base class for the generated repositories
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public abstract class RepositoryBase<T> : IRepository<T> where T : class, ICmObject
	{
		/// <summary>The data reader for accessing the internal object map</summary>
		internal readonly IDataReader m_dataReader;
		/// <summary>The cache</summary>
		protected readonly LcmCache m_cache;

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
		/// </summary>
		/// <param name="cache">The cache.</param>
		/// <param name="dataReader">The data reader.</param>
		/// ------------------------------------------------------------------------------------
		internal RepositoryBase(LcmCache cache, IDataReader dataReader)
		{
			if (cache == null) throw new ArgumentNullException("cache");
			if (dataReader == null) throw new ArgumentNullException("dataReader");

			m_dataReader = dataReader;
			m_cache = cache;
		}

		internal LcmCache Cache {get { return m_cache; }}

		/// <summary>
		/// This class is not currently disposable, but it's convenient for it to be able to tell if the cache is disposed.
		/// </summary>
		internal bool IsDisposed { get { return m_cache == null || m_cache.IsDisposed; } }

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Gets the class ID for this LCM object.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		protected abstract int ClassId { get; }

		/// <summary>
		/// Gets the sync root.
		/// </summary>
		/// <value>The sync root.</value>
		protected object SyncRoot
		{
			get
			{
				return this;
			}
		}

		#region IRepository<T> Members
		/// <summary>
		/// Get the object with the given id. (Deprecated...try to use the ICmObject version.)
		/// </summary>
		/// <param name="id">The Guid id for the object</param>
		/// <returns>The ICmObject of the given id.</returns>
		/// <exception cref="KeyNotFoundException">Thrown if the object does not exist.</exception>
		public T GetObject(Guid id)
		{
			return m_dataReader.GetObject<T>(id);
		}

		/// <summary>
		/// Get the object with the given id.
		/// </summary>
		/// <exception cref="KeyNotFoundException">Thrown if the object does not exist.</exception>
		public T GetObject(ICmObjectId id)
		{
			return m_dataReader.GetObject<T>(id);
		}

		/// <summary>
		/// Get the object with the given HVO.
		/// </summary>
		/// <param name="hvo">The HVO for the object</param>
		/// <returns>The ICmObject of the given HVO.</returns>
		/// <exception cref="KeyNotFoundException">Thrown if the object does not exist.</exception>
		public T GetObject(int hvo)
		{
			return m_dataReader.GetObject<T>(hvo);
		}

		/// <summary>
		/// Try to get the object with the given HVO.
		/// </summary>
		/// <param name="hvo">The HVO for the object</param>
		/// <param name="obj">The returned object, or undefined if it is not found (or of the wrong class)</param>
		/// <returns>True if the object is returned in the out parameter, otherwise false.</returns>
		public bool TryGetObject(int hvo, out T obj)
		{
			ICmObject foundObj;
			if (m_dataReader.TryGetObject(hvo, out foundObj) && foundObj is T)
			{
				obj = (T)foundObj;
				return true;
			}
			obj = default(T);
			return false;
		}

		/// <summary>
		/// Try to get the object with the given Guid.
		/// </summary>
		/// <param name="guid">The Guid for the object</param>
		/// <param name="obj">The returned object, or undefined if it is not found (or of the wrong class)</param>
		/// <returns>True if the object is returned in the out parameter, otherwise false.</returns>
		public bool TryGetObject(Guid guid, out T obj)
		{
			ICmObject foundObj;
			if (m_dataReader.TryGetObject(guid, out foundObj) && foundObj is T)
			{
				obj = (T)foundObj;
				return true;
			}
			obj = default(T);
			return false;
		}

		/// <summary>
		/// Get all instances of the type.
		/// </summary>
		/// <returns>A set of all instances. (There will be zero, or more instances in the Set.)</returns>
		public IEnumerable<T> AllInstances()
		{
			return m_dataReader.AllInstances<T>(ClassId);
		}

		/// <summary>
		/// Get all instances of the specified type.
		/// </summary>
		public IEnumerable<ICmObject> AllInstances(int classId)
		{
			return m_dataReader.AllInstances(classId);
		}

		/// <summary>
		/// Get the count of the objects.
		/// </summary>
		public int Count
		{
			get { return m_dataReader.Count(ClassId); }
		}
		#endregion
	}
}
