using System;
using System.Collections;

namespace Robust
{
	public class History
	{
		private Hashtable table;
		private bool saving;
		
		public History()
		{
			table = new Hashtable();
			saving = false;
		}

		public void CreateToken(string token)
		{
			table.Add(token, new ArrayList());
		}

		public void AddRecord(string token, object data)
		{
			if (!saving) return;
			ArrayList rec = (ArrayList)table[token];
			if (rec != null) rec.Add(data);
		}

		public ArrayList GetRecords(string token)
		{
			return (ArrayList)table[token];
		}

		public void ClearHistory()
		{
			foreach (ArrayList rec in table.Values)
				rec.Clear();
		}

		public void StartHistory()
		{
			ClearHistory();
			saving = true;
		}

		public void StopHistory()
		{
			saving = false;
		}
	}
}