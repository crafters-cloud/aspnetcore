Copies secrets from AzureKeyVault into local user secrets. 

Allows development teams to store secrets on shared development KeyVault and keeps them locally in sync in UserSecrets without manual updates.

Avoids fetching secrets on every start of the application when in development (and paying the performance penalty), but only after a predefined interval.