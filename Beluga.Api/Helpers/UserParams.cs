﻿namespace Sendeazy.Api.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;

        public int pageNumber { get; set; } = 1;

        private int pageSize = 5;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public int UserId { get; set; }

        public string Gender { get; set; }

        public int MinAge { get; set; } = 18;

        public int MaxAge { get; set; } = 99;



    }
}
