using Microsoft.AspNetCore.Mvc;

// to robi że np. metoda Get w controllerze dostaje różne statusy bez wypisywania ich
[assembly: ApiConventionType(typeof(DefaultApiConventions))]