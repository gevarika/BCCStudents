using System;

namespace BCCStudents.Application.Interfaces
{
    // ეს ინტერფეისი წარმოადგენს ბიზნესის კონტრაქტს კავშირის სტატუსის მართვისთვის.
    public interface IConnectionStatusService
    {
        // აბრუნებს კავშირის მიმდინარე სტატუსს
        bool IsConnected { get; }

        // მეთოდი, რომელიც ამოწმებს კავშირს.
        // ის შეიძლება იყოს ასინქრონული, მაგრამ სიმარტივისთვის დავტოვოთ სინქრონულად
        bool CheckConnection();

        // ივენთი, თუ კავშირის სტატუსი შეიცვალა (მაგ., წარუმატებელი მცდელობის შემდეგ)
        event EventHandler ConnectionStatusChanged;
    }
}
