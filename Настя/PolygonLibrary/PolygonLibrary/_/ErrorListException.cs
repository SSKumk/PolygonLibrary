using System;

namespace Robust
{
	public class ErrorListException : Exception
	{
		private readonly string[] errors;

        public ErrorListException(string error)
            : base(error)
        {
            this.errors = new string[] { error };
        }
        
        public ErrorListException(string[] errors) : 
			base(ErrorListException.makeMessage(errors))
		{
			this.errors = errors;
		}

		public string[] Errors 
		{
			get { return errors; }
		}

		private static string makeMessage(string[] errors)
		{
			if (errors == null || errors.Length == 0) return "";
			string msg = errors[0];
			for (int i = 1; i < errors.Length; i++) 
				msg += Environment.NewLine + errors[i];
			return msg;
		}
	}
}
