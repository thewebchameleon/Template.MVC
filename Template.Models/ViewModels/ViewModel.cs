﻿namespace Template.Models.ViewModels
{
    public class ViewModel<TRequest> where TRequest : new()
    {
        public TRequest Request { get; set; }

        public ViewModel(TRequest request)
        {
            Request = request;
        }
    }

    public class ViewModel
    {
        
    }
}
