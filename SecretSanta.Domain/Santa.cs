using System;

namespace SecretSanta.Domain
{
    public record Santa
    {
        public string Name { get; init; }
        public string Email { get; init; }
    }
}